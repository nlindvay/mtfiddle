using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using mtfiddle;
using Serilog;
using Microsoft.Extensions.Azure;

await CreateHostBuilder(args).Build().RunAsync();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddOptions<TypeOptions>();
            services.AddAzureClients(cfg => cfg.AddBlobServiceClient("UseDevelopmentStorage=true").ConfigureOptions(o => o.Diagnostics.IsLoggingEnabled = false));
            services.AddSingleton<ClaimProvider>();
            services.ConfigureMassTransit();
            services.AddHostedService<MockMessageProvider>();
        }).UseSerilog((hostingContext, services, LoggerConfiguration) =>
        {
            LoggerConfiguration.WriteTo.Console();
        });