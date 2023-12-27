using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace mtfiddle;

public class MockMessageProvider : BackgroundService
{
    readonly IBus _bus;
    readonly ClaimCheckProvider _claimProvider;
    readonly OrderProvider _orderProvider;
    readonly ReceiveProvider _receiveProvider;
    readonly TransactionProvider _transactionProvider;
    readonly ILogger<MockMessageProvider> _logger;
    readonly ClaimCheckType _typeOptions;

    public MockMessageProvider(IBus bus, ClaimCheckProvider claimProvider, OrderProvider orderProvider, ReceiveProvider receiveProvider, TransactionProvider transactionProvider, ILogger<MockMessageProvider> logger, IOptions<ClaimCheckType> options)
    {
        _bus = bus;
        _claimProvider = claimProvider;
        _orderProvider = orderProvider;
        _receiveProvider = receiveProvider;
        _transactionProvider = transactionProvider;
        _logger = logger;
        _typeOptions = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int typeSelection = 0;
        int targetTypeSelection = 0;


        while (!stoppingToken.IsCancellationRequested)
        {
            string content = NewId.NextGuid().ToString();
            string reference = ObjectId.GenerateNewId().ToString();
            string type = GetTypeSelection(typeSelection);

            if (type == _typeOptions.EventType)
            {
                string targetType = GetTargetTypeSelection(targetTypeSelection);

                if (targetType == _typeOptions.ReceiveType)
                {
                    var result = await _receiveProvider.GetFirstEnteredId();
                    if (result.IsSuccessful)
                        content = new NewEvent(targetType, result.Value?.ToString(), ReceiveStatus.Arrived).ToJson();
                    else
                        _logger.LogError($"MockMessageProvider: {result.Status} {result.Message}");
                }
                else if (targetType == _typeOptions.OrderType)
                {
                    var result = await _orderProvider.GetFirstEnteredId();
                    if (result.IsSuccessful)
                        content = new NewEvent(targetType, result.Value?.ToString(), OrderStatus.Departed).ToJson();
                    else
                        _logger.LogError($"MockMessageProvider: {result.Status} {result.Message}");
                }
                else if (targetType == _typeOptions.TransactionType)
                {
                    var result = await _transactionProvider.GetFirstUnpaidId();
                    if (result.IsSuccessful)
                        content = new NewEvent(targetType, result.Value?.ToString(), TransactionStatus.Paid).ToJson();
                    else
                        _logger.LogError($"MockMessageProvider: {result.Status} {result.Message}");
                }
                else
                {
                    _logger.LogError($"MockMessageProvider: Invalid target type: {targetType}");

                }

                targetTypeSelection = ++targetTypeSelection == 3 ? 0 : targetTypeSelection++;
            }
            else if (type == _typeOptions.OrderType)
            {
                content = new NewOrder(reference).ToJson();
            }
            else if (type == _typeOptions.ReceiveType)
            {
                content = new NewReceive(reference).ToJson();
            }
            else if (type == _typeOptions.TransactionType)
            {
                content = new NewTransaction(reference).ToJson();
            }
            else
            {
                _logger.LogError($"MockMessageProvider: Invalid type: {type}");
                continue;
            }

            Result<ClaimCheck?> claim = await _claimProvider.CreateAsync(content);

            if (!claim.IsSuccessful)
            {
                _logger.LogError($"MockMessageProvider: {claim.Status} {claim.Message}");
                continue;
            }

            _logger.LogInformation($"MockMessageProvider: Sending {type} Claim Check: {content}");
            await _bus.Publish(claim.Value, x => x.Headers.Set("type", type), stoppingToken);

            await Task.Delay(10000, stoppingToken);

            typeSelection = ++typeSelection == 5 ? 0 : typeSelection;

        }
    }

    protected string GetTypeSelection(int counter) => counter switch
    {
        0 => _typeOptions.OrderType,
        1 => _typeOptions.ReceiveType,
        2 => _typeOptions.TransactionType,
        3 => _typeOptions.EventType,
        4 => _typeOptions.EventType,
        _ => string.Empty
    };

    protected string GetTargetTypeSelection(int counter) => counter switch
    {
        0 => _typeOptions.OrderType,
        1 => _typeOptions.ReceiveType,
        2 => _typeOptions.TransactionType,
        _ => string.Empty
    };
}