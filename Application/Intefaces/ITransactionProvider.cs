using System.Threading.Tasks;

namespace mtfiddle.Application;

public interface ITransactionProvider
{
    Task<Result<Transaction?>> GetAsync(string id);
    Task<Result<Transaction?>> CreateAsync(Transaction transaction);
    Task<Result<Transaction?>> UpdateAsync(Transaction transaction);
}