namespace PrometheusMonitor.TuringSmartScreen.Workers;

using Microsoft.Extensions.Options;

using PrometheusMonitor.TuringSmartScreen.Settings;

internal class Worker : BackgroundService
{
    private readonly ILogger<Worker> log;

    private readonly Setting setting;

    public Worker(
        ILogger<Worker> log,
        IOptions<Setting> setting)
    {
        this.log = log;
        this.setting = setting.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // TODO
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(setting.Interval, stoppingToken);
        }
    }
}
