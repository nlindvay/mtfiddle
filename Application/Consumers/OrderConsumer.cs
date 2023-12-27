using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace mtfiddle;

public class OrderConsumer : IConsumer<ClaimCheck>
{

    readonly ClaimCheckProvider _claimProvider;
    readonly OrderProvider _orderProvider;
    readonly ILogger<IConsumer<ClaimCheck>> _logger;


    public OrderConsumer(ClaimCheckProvider claimCheckProvider, OrderProvider orderProvider, ILogger<IConsumer<ClaimCheck>> logger)
    {
        _claimProvider = claimCheckProvider;
        _orderProvider = orderProvider;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ClaimCheck> context)
    {
        var content = await _claimProvider.GetAsync(context.Message.Uri);
        
        NewOrder newOrder = content.Value.Content;
        Order order = newOrder;

        await _orderProvider.CreateAsync(order);

        _logger.LogInformation("OrderConsumer: {Type} {Content}", context.Headers.Get<string>("type"), content);
    }
}