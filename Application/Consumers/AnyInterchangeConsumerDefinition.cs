using MassTransit;
using RabbitMQ.Client;

namespace mtfiddle;

public class AnyConsumerDefinition : ConsumerDefinition<AnyInterchangeConsumer>
{
    public AnyConsumerDefinition()
    {
        EndpointName = $"mtfiddle.interchange.any";
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AnyInterchangeConsumer> consumerConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.ConfigureConsumeTopology = false;

        if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbitMq)
        {
            rabbitMq.Bind<ClaimCheck>(x =>
            {
                x.ExchangeType = ExchangeType.Headers;
            });
        }


        if (endpointConfigurator is IServiceBusReceiveEndpointConfigurator azureServiceBus)
        {
            azureServiceBus.Subscribe<ClaimCheck>("mtfiddle.any");
        }
    }
}
