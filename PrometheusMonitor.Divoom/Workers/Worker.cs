namespace PrometheusMonitor.Divoom.Workers;

using Microsoft.Extensions.Options;

using PrometheusMonitor.Divoom.Settings;

internal class Worker : BackgroundService
{
    private readonly ILogger<Worker> log;

    private readonly PrometheusSetting prometheusSetting;

    private readonly DivoomSetting divoomSetting;

    public Worker(
        ILogger<Worker> log,
        IOptions<PrometheusSetting> prometheusSetting,
        IOptions<DivoomSetting> divoomSetting)
    {
        this.log = log;
        this.prometheusSetting = prometheusSetting.Value;
        this.divoomSetting = divoomSetting.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // TODO
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}
