namespace PrometheusMonitor.Divoom.Settings;

internal sealed class Setting
{
    public string PrometheusUrl { get; set; } = default!;

    public int Interval { get; set; } = 10_000;
}
