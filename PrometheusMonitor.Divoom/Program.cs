using System.Runtime.InteropServices;
using System.Runtime;

using PrometheusMonitor.Divoom;
using PrometheusMonitor.Divoom.Settings;
using PrometheusMonitor.Divoom.Workers;

using Serilog;

Directory.SetCurrentDirectory(AppContext.BaseDirectory);

var builder = Host.CreateApplicationBuilder(args);

// Service
builder.Services
    .AddWindowsService()
    .AddSystemd();

// Logging
builder.Logging.ClearProviders();
builder.Services.AddSerilog(options =>
{
    options.ReadFrom.Configuration(builder.Configuration);
});

// Setting
builder.Services.Configure<PrometheusSetting>(builder.Configuration.GetSection("Prometheus"));
builder.Services.Configure<DivoomSetting>(builder.Configuration.GetSection("Divoom"));

// Worker
builder.Services.AddHostedService<Worker>();

// Build
var host = builder.Build();

// Startup information
var log = host.Services.GetRequiredService<ILogger<Program>>();
ThreadPool.GetMinThreads(out var workerThreads, out var completionPortThreads);
log.InfoStartup();
log.InfoStartupSettingsRuntime(RuntimeInformation.OSDescription, RuntimeInformation.FrameworkDescription, RuntimeInformation.RuntimeIdentifier);
log.InfoStartupSettingsGC(GCSettings.IsServerGC, GCSettings.LatencyMode, GCSettings.LargeObjectHeapCompactionMode);
log.InfoStartupSettingsThreadPool(workerThreads, completionPortThreads);
log.InfoStartupSettingsEnvironment(typeof(Program).Assembly.GetName().Version, Environment.CurrentDirectory);

// Run
await host.RunAsync();
