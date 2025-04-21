namespace PrometheusMonitor.TuringSmartScreen.Settings;

internal sealed class Setting
{
    public int Interval { get; set; } = 10_000;

    public string PrometheusUrl { get; set; } = default!;
}
