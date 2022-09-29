using System.Threading;
using System.Threading.Tasks;
using AtomicAssetsClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SalesDumper;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureLogging(
    l => l.AddSimpleConsole(
        options =>
        {
            options.IncludeScopes = false;
            options.SingleLine = true;
            options.TimestampFormat = "hh:mm:ss ";
        }));

builder.UseConsoleLifetime();
builder.ConfigureServices((context, services) =>
{
    services.AddHttpClient();
    services.Configure<AtomicClientOptions>(context.Configuration.GetSection("AtomicClientOptions"));
    services.AddSingleton<IAtomicClient, AtomicClient>();
    services.AddSingleton<DumpService>();
});

var app = builder.Build();

var dumper = app.Services.GetRequiredService<DumpService>();

await dumper.Run().ConfigureAwait(false);

// wait while logger flushes
await Task.Delay(1000).ConfigureAwait(false);
