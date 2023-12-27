using Azure.Messaging.ServiceBus.Administration;
using MassTransit;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace mtfiddle;

public class ProductConsumerDefinition : ConsumerDefinition<ProductConsumer>
{
    readonly string _type;

    public ProductConsumerDefinition(IOptions<TypeOptions> routeOptions)
    {
        _type = routeOptions.Value.ProductType;
        EndpointName = $"mtfiddle.interchange.{_type}";
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ProductConsumer> consumerConfigurator, IRegistrationContext context)
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
