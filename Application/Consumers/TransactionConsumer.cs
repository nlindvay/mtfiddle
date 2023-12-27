using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace mtfiddle;

public class TransactionConsumer : IConsumer<ClaimCheck>
{

    readonly ClaimCheckProvider _claimProvider;
    readonly TransactionProvider _transactionProvider;
    readonly ILogger<IConsumer<ClaimCheck>> _logger;


    public TransactionConsumer(ClaimCheckProvider claimCheckProvider, TransactionProvider transactionProvider, ILogger<IConsumer<ClaimCheck>> logger)
    {
        _claimProvider = claimCheckProvider;
        _transactionProvider = transactionProvider;
        _logger = logger;
    }


    public async Task Consume(ConsumeContext<ClaimCheck> context)
    {
        var content = await _claimProvider.GetAsync(context.Message.Uri);

        NewTransaction newTransaction = content.Value.Content;
        Transaction transaction = newTransaction;

        await _transactionProvider.CreateAsync(transaction);

        _logger.LogInformation("TransactionConsumer: {Type} {Content}", context.Headers.Get<string>("type"), content);
    }
}