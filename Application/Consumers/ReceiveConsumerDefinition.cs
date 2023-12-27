using Azure.Messaging.ServiceBus.Administration;
using MassTransit;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace mtfiddle;

public class ReceiveConsumerDefinition : ConsumerDefinition<ReceiveConsumer>
{
    readonly string _type;

    public ReceiveConsumerDefinition(IOptions<ClaimCheckType> routeOptions)
    {
        _type = routeOptions.Value.ReceiveType;
        EndpointName = $"mtfiddle.interchange.{_type}";
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ReceiveConsumer> consumerConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.ConfigureConsumeTopology = false;

        if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rmq)
        {
            rmq.Bind<ClaimCheck>(x =>
            {
                x.ExchangeType = ExchangeType.Headers;
                x.SetBindingArgument("x-match", "all");
                x.SetBindingArgument("type", _type);
            });
        }


        if (endpointConfigurator is IServiceBusReceiveEndpointConfigurator asb)
        {
            asb.Subscribe<ClaimCheck>(_type, x =>
            {
                x.Rule = new CreateRuleOptions()
                {
                    Filter = new SqlRuleFilter($"type='{_type}'")
                };
            });
        }
    }
}