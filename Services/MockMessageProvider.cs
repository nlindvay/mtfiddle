using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace mtfiddle;

public class MockMessageProvider : BackgroundService
{
    readonly IBus _bus;
    readonly ClaimProvider _claimProvider;
    readonly ILogger<MockMessageProvider> _logger;
    readonly TypeOptions _typeOptions;
    public MockMessageProvider(IBus bus, ClaimProvider claimProvider, ILogger<MockMessageProvider> logger, IOptions<TypeOptions> options)
    {
        _bus = bus;
        _claimProvider = claimProvider;
        _logger = logger;
        _typeOptions = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            string content = new Random().Next(0, 100).ToString();
            string type = GetRandomType();
            Result<ClaimCheck?> claim = await _claimProvider.CreateAsync(content);

            if (!claim.IsSuccessful)
            {
                _logger.LogError($"MockMessageProvider: {claim.Status} {claim.Message}");
                await Task.Delay(2000, stoppingToken);
                continue;
            }

            _logger.LogInformation($"MockMessageProvider: Sending {type} Claim Check: {content}");
            await _bus.Publish(claim.Value, x => x.Headers.Set("type", _typeOptions.OrderType), stoppingToken);


            await Task.Delay(2000, stoppingToken);
        }
    }

    protected string GetRandomType() => new Random().Next(0, 4) switch
    {
        0 => _typeOptions.OrderType,
        1 => _typeOptions.ReceiveType,
        2 => _typeOptions.ProductType,
        3 => _typeOptions.TransactionType,
        4 => _typeOptions.EventType,
        _ => string.Empty
    };
}