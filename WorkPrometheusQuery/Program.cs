namespace WorkPrometheusQuery;

using System.Diagnostics;

using Newtonsoft.Json.Linq;

internal static class Program
{
    private static readonly HttpClient Client = new();
    private const string PrometheusUrl = "http://prometheus-server:9090/api/v1/query?query=";

    public static async Task Main()
    {
        // 温度
        await GetLatestMetrics("sensor_temperature");
        await GetLatestMetrics("sensor_power");
        await GetLatestMetrics("hardware_cpu_power{name=~\"^(Package|CPU Package)$\"}");
        await GetLatestMetrics("hardware_cpu_temperature{name=~\"^(CPU Package|Core \\\\(Tctl/Tdie\\\\))$\"}");
        await GetLatestMetrics("hardware_gpu_temperature{name=\"GPU Core\"}");

        await GetLatestMetrics("hardware_cpu_load{name=\"CPU Total\"}");
        await GetLatestMetrics("hardware_memory_load{type=\"physical\"}");
        await GetLatestMetrics("hardware_gpu_load{name=\"GPU Core\"}");
        await GetLatestMetrics("100 - system_disk_free{name!~\"_Total|^Harddisk.*\"}");

        await GetLatestMetrics("smart_nvme_value{smart_id=\"temperature\"}", "drive");
        await GetLatestMetrics("smart_generic_value{smart_id=\"C2\"} % 256", "drive");
        await GetLatestMetrics("topk(1, smart_nvme_value{smart_id=\"temperature\"}) by (host)", "drive");
    }

    private static async Task GetLatestMetrics(string query, string label = "name")
    {
        var url = new Uri(PrometheusUrl + Uri.EscapeDataString(query));
        var response = await Client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request to Prometheus failed with status code {response.StatusCode}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        var jsonResponse = JObject.Parse(responseString);

        var results = jsonResponse["data"]?["result"];
        if (results is not null)
        {
            foreach (var result in results)
            {
                var metric = result["metric"]?.ToObject<Dictionary<string, object>>() ?? [];
                var value = result["value"];
                if ((value is not null) && (value is JArray { Count: >= 2 }))
                {
                    var key = metric.TryGetValue("__name__", out var v) ? v : "-";
                    var host = metric.TryGetValue("host", out v) ? v : "-";
                    var name = metric.TryGetValue(label, out v) ? v : "-";
                    var timestamp = DateTimeOffset.FromUnixTimeSeconds(value[0]!.Value<long>()).LocalDateTime;
                    var metricValue = value[1]!.Value<double>();

                    Debug.WriteLine($"{timestamp:yyyy-MM-dd HH:mm:ss} : {key} : {host} {name} : {metricValue}");
                }
            }
        }
    }
}
