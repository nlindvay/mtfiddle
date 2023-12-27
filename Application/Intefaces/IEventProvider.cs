using System.Threading.Tasks;

namespace mtfiddle.Application;

public interface IEventProvider
{
    Task<Result<Event?>> CreateAsync(Event Event);
}