using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using mtfiddle.Application;

namespace mtfiddle;

public class EventConsumer : IConsumer<ClaimCheck>
{
    readonly IClaimCheckProvider _claimProvider;
    readonly IEventProvider _eventProvider;
    readonly IReceiveProvider _receiveProvider;
    readonly IOrderProvider _orderProvider;
    readonly ITransactionProvider _transactionProvider;
    readonly ILogger<IConsumer<ClaimCheck>> _logger;

    public EventConsumer(IClaimCheckProvider claimProvider, IEventProvider eventProvider, IReceiveProvider receiveProvider, IOrderProvider orderProvider, ITransactionProvider transactionProvider, ILogger<IConsumer<ClaimCheck>> logger)
    {
        _claimProvider = claimProvider;
        _eventProvider = eventProvider;
        _receiveProvider = receiveProvider;
        _orderProvider = orderProvider;
        _transactionProvider = transactionProvider;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ClaimCheck> context)
    {
        var result = await _claimProvider.GetAsync(context.Message.Uri!);
        NewEvent newEvent = result.Value!.Content!;
        Event entity = newEvent;

        await _eventProvider.CreateAsync(entity);

        if (entity.TargetType == _claimProvider.TypeOptions.ReceiveType)
        {
            Result<Receive?> receiveResult = await _receiveProvider.GetAsync(entity.TargetId);

            if (!receiveResult.IsSuccessful)
                throw new InvalidOperationException(receiveResult.Message);

            Receive receive = receiveResult.Value!;
            receive.ApplyEventUpdates(entity);
            await _receiveProvider.UpdateAsync(receive);
        }
        else if (entity.TargetType == _claimProvider.TypeOptions.OrderType)
        {
            Result<Order?> orderResult = await _orderProvider.GetAsync(entity.TargetId);

            if (!orderResult.IsSuccessful)
                throw new InvalidOperationException(orderResult.Message);

            Order order = orderResult.Value!;
            order.ApplyEventUpdates(entity);
            await _orderProvider.UpdateAsync(order);
        }
        else if (entity.TargetType == _claimProvider.TypeOptions.TransactionType)
        {
            Result<Transaction?> transactionResult = await _transactionProvider.GetAsync(entity.TargetId);

            if (!transactionResult.IsSuccessful)
                throw new InvalidOperationException(transactionResult.Message);

            Transaction transaction = transactionResult.Value!;
            transaction.ApplyEventUpdates(entity);
            await _transactionProvider.UpdateAsync(transaction);
        }
        else
        {
            throw new InvalidOperationException("Unknown entity type");
        }

        _logger.LogInformation("EventConsumer: {Type} {Content}", context.Headers.Get<string>("type"), result);
    }
}