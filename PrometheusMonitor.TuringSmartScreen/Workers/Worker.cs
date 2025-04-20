namespace PrometheusMonitor.TuringSmartScreen.Workers;

using Microsoft.Extensions.Options;

using PrometheusMonitor.TuringSmartScreen.Settings;

internal class Worker : BackgroundService
{
    private readonly ILogger<Worker> log;

    private readonly PrometheusSetting prometheusSetting;

    private readonly ScreenSetting screenSetting;

    public Worker(
        ILogger<Worker> log,
        IOptions<PrometheusSetting> prometheusSetting,
        IOptions<ScreenSetting> screenSetting)
    {
        this.log = log;
        this.prometheusSetting = prometheusSetting.Value;
        this.screenSetting = screenSetting.Value;
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
