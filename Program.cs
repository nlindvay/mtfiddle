using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using mtfiddle;
using Serilog;
using Microsoft.Extensions.Azure;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;

await CreateHostBuilder(args).Build().RunAsync();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            var mongoClient = new MongoClient(MongoClientSettings.FromConnectionString("mongodb://localhost:27017"));
            services.AddSingleton(mongoClient.GetDatabase("mtfiddle"));
            services.AddSingleton<MongoDbProvider>();
            services.AddDbContext<AppDbContext>(opts => opts.UseMongoDB(mongoClient, "mtfiddle"), ServiceLifetime.Singleton);
            services.AddSingleton<EventProvider>();
            services.AddSingleton<ReceiveProvider>();
            services.AddSingleton<OrderProvider>();
            services.AddSingleton<TransactionProvider>();
            services.AddOptions<ClaimCheckType>();
            services.AddAzureClients(cfg => cfg.AddBlobServiceClient("UseDevelopmentStorage=true").ConfigureOptions(o => o.Diagnostics.IsLoggingEnabled = false));
            services.AddSingleton<ClaimCheckProvider>();
            services.ConfigureMassTransit();
            services.AddHostedService<MockMessageProvider>();
        }).UseSerilog((hostingContext, services, LoggerConfiguration) =>
        {
            LoggerConfiguration.WriteTo.Console();
        });