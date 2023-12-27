using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using mtfiddle.Application;

namespace mtfiddle;

public class EventProvider : IEventProvider
{
    private readonly AppDbContext _context;
    private readonly ILogger<EventProvider> _logger;

    public EventProvider(AppDbContext context, ILogger<EventProvider> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Event?>> CreateAsync(Event Event)
    {
        _logger.LogInformation("Create Event: {Id}", Event.Id);
        _context.Events.Add(Event);

        if (_context.SaveChanges() == 0)
            return Result<Event?>.Error("Unable to create Event.");

        return Result<Event>.Ok(Event);
    }
}