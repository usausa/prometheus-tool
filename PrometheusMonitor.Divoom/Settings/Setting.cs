namespace PrometheusMonitor.Divoom.Settings;

internal sealed class QueryEntry
{
    public string[] CpuUsed { get; set; } = default!;

    public string[] GpuUsed { get; set; } = default!;

    public string[] CpuTemperature { get; set; } = default!;

    public string[] GpuTemperature { get; set; } = default!;

    public string[] MemoryUsed { get; set; } = default!;

    public string[] DiskTemperature { get; set; } = default!;
}

internal sealed class FormatEntry
{
    public string CpuUsed { get; set; } = "{0:F0} %";

    public string GpuUsed { get; set; } = "{0:F0} %";

    public string CpuTemperature { get; set; } = "{0:F0} C";

    public string GpuTemperature { get; set; } = "{0:F0} C";

    public string MemoryUsed { get; set; } = "{0:F0} %";

    public string DiskTemperature { get; set; } = "{0:F0} C";
}

internal sealed class NodeEntry
{
    public string Name { get; set; } = default!;

    public int? Index { get; set; }
}

internal sealed class Setting
{
    public int Interval { get; set; } = 10_000;

    public string PrometheusUrl { get; set; } = default!;

    public string DivoomHost { get; set; } = default!;

    public int? LcdId { get; set; }

    public QueryEntry Query { get; set; } = default!;

    public FormatEntry Format { get; set; } = new();

    public string NodeKey { get; set; } = default!;

    public NodeEntry[] Node { get; set; } = default!;
}
