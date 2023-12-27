using System.Threading.Tasks;

namespace mtfiddle.Application;

public interface IReceiveProvider
{
    Task<Result<Receive?>> GetAsync(string id);
    Task<Result<Receive?>> CreateAsync(Receive receive);
    Task<Result<Receive?>> UpdateAsync(Receive receive);
}