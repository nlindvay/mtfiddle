using System;
using System.Reflection;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace mtfiddle;

public static class RabbitMqMassTransitConfigExtensions
{
    public static void ConfigureMessageTopology(this IRabbitMqBusFactoryConfigurator configurator)
    {
        configurator.Message<ClaimCheck>(x => x.SetEntityName("mtfiddle.claimcheck"));

        configurator.Publish<ClaimCheck>(x =>
        {
            x.ExchangeType = ExchangeType.Headers;
        });
    }

    public static void ConfigureMassTransit(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            x.SetKebabCaseEndpointNameFormatter();
            x.SetInMemorySagaRepositoryProvider();
            x.AddConsumers(entryAssembly);
            x.AddSagaStateMachines(entryAssembly);
            x.AddSagas(entryAssembly);
            x.AddActivities(entryAssembly);
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ConfigureMessageTopology();
                cfg.ConfigureEndpoints(context);
                cfg.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)));
                cfg.UseRateLimit(1000, TimeSpan.FromMinutes(2));
                cfg.ConcurrentMessageLimit = 50;
                cfg.PrefetchCount = 100;
                cfg.UseInMemoryOutbox(context);
            });

        });
    }
}