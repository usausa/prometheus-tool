using System.Runtime.InteropServices;
using System.Runtime;

using PrometheusMonitor.Divoom;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

var log = host.Services.GetRequiredService<ILogger<Program>>();
ThreadPool.GetMinThreads(out var workerThreads, out var completionPortThreads);
log.InfoStartup();
log.InfoStartupSettingsRuntime(RuntimeInformation.OSDescription, RuntimeInformation.FrameworkDescription, RuntimeInformation.RuntimeIdentifier);
log.InfoStartupSettingsGC(GCSettings.IsServerGC, GCSettings.LatencyMode, GCSettings.LargeObjectHeapCompactionMode);
log.InfoStartupSettingsThreadPool(workerThreads, completionPortThreads);
log.InfoStartupSettingsEnvironment(typeof(Program).Assembly.GetName().Version, Environment.CurrentDirectory);

host.Run();
