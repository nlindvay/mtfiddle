using System.Threading.Tasks;

namespace mtfiddle.Application;

public interface IClaimCheckProvider
{
    Task<Result<ClaimCheck?>> GetAsync(string uri);
    Task<Result<ClaimCheck?>> CreateAsync(string content);

    ClaimCheckType ClaimCheckType { get; }
}