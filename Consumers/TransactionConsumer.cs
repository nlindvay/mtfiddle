using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace mtfiddle;

public class TransactionConsumer : IConsumer<ClaimCheck>
{

    readonly ClaimProvider _claimProvider;
    readonly ILogger<IConsumer<ClaimCheck>> _logger;


    public TransactionConsumer(ClaimProvider claimCheckProvider, ILogger<IConsumer<ClaimCheck>> logger)
    {
        _claimProvider = claimCheckProvider;
        _logger = logger;
    }


    public async Task Consume(ConsumeContext<ClaimCheck> context)
    {
        var content = await _claimProvider.GetAsync(context.Message.Uri);
        _logger.LogInformation("TransactionConsumer: {Type} {Content}", context.Headers.Get<string>("type"), content);
    }
}