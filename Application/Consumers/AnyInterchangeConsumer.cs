using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using mtfiddle.Application;

namespace mtfiddle;

public class AnyInterchangeConsumer : IConsumer<ClaimCheck>
{

    readonly IClaimCheckProvider _claimProvider;
    readonly ILogger<IConsumer<ClaimCheck>> _logger;


    public AnyInterchangeConsumer(IClaimCheckProvider claimCheckProvider, ILogger<IConsumer<ClaimCheck>> logger)
    {
        _claimProvider = claimCheckProvider;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ClaimCheck> context)
    {
        var content = await _claimProvider.GetAsync(context.Message.Uri);

        
        _logger.LogInformation("AnyInterchangeConsumer: {Type} {Content}", context.Headers.Get<string>("type"), content);
    }
}
