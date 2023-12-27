using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace mtfiddle;

public class TransactionProvider
{
    private readonly AppDbContext _context;
    private readonly ILogger<TransactionProvider> _logger;

    public TransactionProvider(AppDbContext context, ILogger<TransactionProvider> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Transaction?>> CreateAsync(Transaction transaction)
    {
        _logger.LogInformation("Create transaction: {Id}", transaction.Id);
        _context.Transactions.Add(transaction);

        if (_context.SaveChanges() == 0)
            return Result<Transaction?>.Error("Unable to create transaction.");

        return Result<Transaction?>.Ok(transaction);
    }

    public async Task<Result<Transaction?>> GetAsync(string id)
    {
        _logger.LogInformation("Get transaction: {Id}", id);
        var transaction = _context.Transactions.FirstOrDefaultAsync(x => x.Id == ObjectId.Parse(id)).Result;

        if (transaction == null)
            return Result<Transaction?>.Error("Unable to get transaction.");

        return Result<Transaction?>.Ok(transaction);
    }

    public async Task<Result<ObjectId?>> GetFirstUnpaidId()
    {
        _logger.LogInformation("Get first entered transaction id.");
        var transaction = _context.Transactions.FirstOrDefaultAsync(x => x.Status == TransactionStatus.Unpaid).Result;

        if (transaction == null)
            return Result<ObjectId?>.Error("Unable to get first entered transaction id.");

        return Result<ObjectId?>.Ok(transaction.Id);
    }

    public async Task<Result<Transaction?>> UpdateAsync(Transaction transaction)
    {
        _logger.LogInformation("Update transaction: {Id}", transaction.Id);
        _context.Transactions.Update(transaction);

        if (_context.SaveChanges() == 0)
            return Result<Transaction?>.Error("Unable to update transaction.");

        return Result<Transaction?>.Ok(transaction);
    }
}