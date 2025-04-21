namespace PrometheusMonitor.Divoom.Workers;

using System.Buffers;
using System.Globalization;

using global::Divoom.Client;

using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

using PrometheusMonitor.Divoom.Settings;

internal sealed class Worker : BackgroundService
{
    private const string DataNothing = "-";

    private readonly ILogger<Worker> log;

    private readonly Setting setting;

    private readonly HttpClient httpClient;

    private readonly DivoomClient divoomClient;

    private readonly MonitorParameter[] parameters;

    private readonly double[] valueBffer;

    public Worker(
        ILogger<Worker> log,
        IOptions<Setting> setting)
    {
        this.log = log;
        this.setting = setting.Value;

        httpClient = new()
        {
            BaseAddress = new Uri(this.setting.PrometheusUrl),
            Timeout = TimeSpan.FromSeconds(5)
        };

        divoomClient = new DivoomClient(this.setting.DivoomHost);

        parameters = this.setting.Node.Select(static x => new MonitorParameter { Lcd = x.Index }).ToArray();
        valueBffer = new double[parameters.Length];
    }

    public override void Dispose()
    {
        base.Dispose();
        httpClient.Dispose();
        divoomClient.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var initialized = false;
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(setting.Interval));
        try
        {
            do
            {
#pragma warning disable CA1031
                if (!initialized)
                {
                    if (await SelectClockAsync())
                    {
                        initialized = true;
                    }
                }

                if (initialized)
                {
                    await UpdateParameterAsync();

                    if (!await UpdateMonitorAsync())
                    {
                        initialized = false;
                    }
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
    }

    private async ValueTask<bool> SelectClockAsync()
    {
#pragma warning disable CA1031
        try
        {
            foreach (var node in setting.Node)
            {
                var result = await divoomClient.SelectClockIdAsync(625, setting.LcdId, node.Index);
                if ((result.ErrorCode != 0) || !String.IsNullOrEmpty(result.ErrorMessage))
                {
                    log.LogWarning("TODO");
                    return false;
                }
            }

            return true;
        }
        catch (Exception e)
        {
            log.ErrorUnknownException(e);
        }
#pragma warning restore CA1031

        return true;
    }

    private async ValueTask<bool> UpdateMonitorAsync()
    {
#pragma warning disable CA1031
        try
        {
            var result = await divoomClient.UpdatePcMonitorAsync(parameters);
            if ((result.ErrorCode != 0) || !String.IsNullOrEmpty(result.ErrorMessage))
            {
                log.LogWarning("TODO");
            }

            return true;
        }
        catch (Exception e)
        {
            log.ErrorUnknownException(e);
        }
#pragma warning restore CA1031

        return true;
    }

    private async ValueTask UpdateParameterAsync()
    {
        foreach (var parameter in parameters)
        {
            parameter.CpuUsed = DataNothing;
            parameter.GpuUsed = DataNothing;
            parameter.CpuTemperature = DataNothing;
            parameter.GpuTemperature = DataNothing;
            parameter.MemoryUsed = DataNothing;
            parameter.DiskTemperature = DataNothing;
        }

#pragma warning disable CA1031
        try
        {
            await UpdateMetricsAsync(setting.Query.CpuUsed, "{0:F1}", static (p, v) => p.CpuUsed = v);
            await UpdateMetricsAsync(setting.Query.GpuUsed, "{0:F1}", static (p, v) => p.GpuUsed = v);
            await UpdateMetricsAsync(setting.Query.CpuTemperature, "{0:F1}", static (p, v) => p.CpuTemperature = v);
            await UpdateMetricsAsync(setting.Query.GpuTemperature, "{0:F1}", static (p, v) => p.GpuTemperature = v);
            await UpdateMetricsAsync(setting.Query.MemoryUsed, "{0:F0}", static (p, v) => p.MemoryUsed = v);
            await UpdateMetricsAsync(setting.Query.DiskTemperature, "{0:F0}", static (p, v) => p.DiskTemperature = v);
        }
        catch (Exception e)
        {
            log.ErrorUnknownException(e);
        }
#pragma warning restore CA1031
    }

    private async ValueTask UpdateMetricsAsync(IEnumerable<string> queries, string format, Action<MonitorParameter, string> setter)
    {
        valueBffer.AsSpan().Fill(double.MinValue);

        foreach (var query in queries)
        {
#pragma warning disable CA2234
            var response = await httpClient.GetAsync("api/v1/query?query=" + Uri.EscapeDataString(query));
#pragma warning restore CA2234
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var jsonResponse = JObject.Parse(responseString);

            var results = jsonResponse["data"]?["result"];
            if (results is not null)
            {
                foreach (var result in results)
                {
                    var metric = result["metric"]?.ToObject<Dictionary<string, object>>() ?? [];
                    var value = result["value"];
                    if ((value is JArray { Count: >= 2 }) &&
                        metric.TryGetValue("host", out var h))
                    {
                        var host = h.ToString();
                        for (var i = 0; i < setting.Node.Length; i++)
                        {
                            var node = setting.Node[i];
                            if (host == node.Name)
                            {
                                var metricValue = value[1]!.Value<double>();
                                valueBffer[i] = Math.Max(valueBffer[i], metricValue);
                                break;
                            }
                        }
                    }
                }
            }
        }

        for (var i = 0; i < setting.Node.Length; i++)
        {
            var value = valueBffer[i];
            if (value > double.MinValue)
            {
                setter(parameters[i], String.Format(CultureInfo.InvariantCulture, format, value));
            }
        }
    }
}
