using Azure.Messaging.ServiceBus.Administration;
using MassTransit;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace mtfiddle;

public class OrderConsumerDefinition : ConsumerDefinition<OrderConsumer>
{
    readonly string _type;

    public OrderConsumerDefinition(IOptions<TypeOptions> routeOptions)
    {
        _type = routeOptions.Value.OrderType;
        EndpointName = $"mtfiddle.interchange.{_type}";
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<OrderConsumer> consumerConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.ConfigureConsumeTopology = false;

        if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbitMq)
        {

            rabbitMq.Bind<ClaimCheck>(x =>
            {
                x.ExchangeType = ExchangeType.Headers;
                x.SetBindingArgument("x-match", "all");
                x.SetBindingArgument("type", _type);
            });
        }


        if (endpointConfigurator is IServiceBusReceiveEndpointConfigurator azureServiceBus)
        {
            azureServiceBus.Subscribe<ClaimCheck>(_type, x =>
            {
                x.Rule = new CreateRuleOptions()
                {
                    Filter = new SqlRuleFilter($"type='{_type}'")
                };
            });
        }
    }

}
