namespace PrometheusMonitor.Divoom.Settings;

internal sealed class Setting
{
    public int Interval { get; set; } = 10_000;

    public string PrometheusUrl { get; set; } = default!;

    public string DivoomHost { get; set; } = default!;

    public int? LcdId { get; set; }

    internal sealed class QueryEntry
    {
        public string[] CpuUsed { get; set; } = default!;

        public string[] GpuUsed { get; set; } = default!;

        public string[] CpuTemperature { get; set; } = default!;

        public string[] GpuTemperature { get; set; } = default!;

        public string[] MemoryUsed { get; set; } = default!;

        public string[] DiskTemperature { get; set; } = default!;
    }

    public QueryEntry Query { get; set; } = default!;

    public string NodeKey { get; set; } = default!;

    internal sealed class NodeEntry
    {
        public string Name { get; set; } = default!;

        public int? Index { get; set; }
    }

    public NodeEntry[] Node { get; set; } = default!;
}
