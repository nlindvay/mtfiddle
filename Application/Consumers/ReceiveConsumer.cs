using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace mtfiddle;

public class ReceiveConsumer : IConsumer<ClaimCheck>
{
    readonly ClaimCheckProvider _claimProvider;
    readonly ReceiveProvider _receiveProvider;
    readonly ILogger<IConsumer<ClaimCheck>> _logger;


    public ReceiveConsumer(ClaimCheckProvider claimCheckProvider, ReceiveProvider receiveProvider, ILogger<IConsumer<ClaimCheck>> logger)
    {
        _claimProvider = claimCheckProvider;
        _receiveProvider = receiveProvider;
        _logger = logger;
    }


    public async Task Consume(ConsumeContext<ClaimCheck> context)
    {
        var content = await _claimProvider.GetAsync(context.Message.Uri);
        NewReceive newReceive = content.Value.Content;
        Receive receive = newReceive;

        await _receiveProvider.CreateAsync(receive);

        _logger.LogInformation("ReceiveConsumer: {Type} {Content}", context.Headers.Get<string>("type"), content);
    }
}

