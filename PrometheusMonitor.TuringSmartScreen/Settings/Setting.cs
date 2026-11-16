namespace PrometheusMonitor.TuringSmartScreen.Settings;

internal sealed class QueryEntry
{
    public string Name { get; set; } = default!;

    public string Query { get; set; } = default!;
}

internal sealed class ThresholdEntry
{
    public string DefaultText { get; set; } = default!;

    public string DefaultColor { get; set; } = default!;

    public string WarningText { get; set; } = default!;

    public string WarningColor { get; set; } = default!;

    public string CriticalText { get; set; } = default!;

    public string CriticalColor { get; set; } = default!;

    public string LostText { get; set; } = default!;

    public string LostColor { get; set; } = default!;
}

internal sealed class NodeEntry
{
    public string Query { get; set; } = default!;

    public Dictionary<string, string> Tag { get; } = [];

    public string NameKey { get; set; } = default!;

    public string Format { get; set; } = default!;
}

internal sealed class NodeEntryGroup
{
    public NodeEntry[] Node { get; set; } = default!;
}

internal sealed class ScreenEntry
{
    public string Port { get; set; } = default!;

    public string Type { get; set; } = default!;
}

internal sealed class LayoutEntry
{
    public string Flow { get; set; } = default!;

    public int Width { get; set; }

    public int Height { get; set; }

    public int FontSize { get; set; }
}

internal sealed class Setting
{
    public int Interval { get; set; } = 10_000;

    public string PrometheusUrl { get; set; } = default!;

    public ScreenEntry Screen { get; set; } = default!;

    public QueryEntry[] Query { get; set; } = default!;

    public LayoutEntry Layout { get; set; } = default!;

    public ThresholdEntry Threshold { get; set; } = default!;

    public NodeEntryGroup[] Group { get; set; } = default!;
}
