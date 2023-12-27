using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace mtfiddle;

public class ProductConsumer : IConsumer<ClaimCheck>
{
    readonly ClaimProvider _claimProvider;
    readonly ILogger<IConsumer<ClaimCheck>> _logger;


    public ProductConsumer(ClaimProvider claimCheckProvider, ILogger<IConsumer<ClaimCheck>> logger)
    {
        _claimProvider = claimCheckProvider;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ClaimCheck> context)
    {
        var content = await _claimProvider.GetAsync(context.Message.Uri);
        _logger.LogInformation("ProductConsumer: {Type} {Content}", context.Headers.Get<string>("type"), content);
    }
}