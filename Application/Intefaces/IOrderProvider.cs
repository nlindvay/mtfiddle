using System.Threading.Tasks;

namespace mtfiddle.Application;

public interface IOrderProvider
{
    Task<Result<Order?>> GetAsync(string id);
    Task<Result<Order?>> CreateAsync(Order order);
    Task<Result<Order?>> UpdateAsync(Order order);
}