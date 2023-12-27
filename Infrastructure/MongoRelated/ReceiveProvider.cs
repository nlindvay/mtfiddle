using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using mtfiddle.Application;

namespace mtfiddle;

public class ReceiveProvider : IReceiveProvider
{
    private readonly AppDbContext _context;
    private readonly ILogger<ReceiveProvider> _logger;

    public ReceiveProvider(AppDbContext context, ILogger<ReceiveProvider> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Receive?>> CreateAsync(Receive receive)
    {
        _logger.LogInformation("Create Receive: {Id}", receive.Id);
        _context.Receives.Add(receive);

        if (_context.SaveChanges() == 0)
            return Result<Receive?>.Error("Unable to create receive.");

        return Result<Receive?>.Ok(receive);
    }

    public async Task<Result<Receive?>> GetAsync(string id)
    {
        _logger.LogInformation("Get receive: {Id}", id);
        var receive = _context.Receives.FirstOrDefaultAsync(x => x.Id == ObjectId.Parse(id)).Result;

        if (receive == null)
            return Result<Receive?>.Error("Unable to get receive.");

        return Result<Receive?>.Ok(receive);
    }

    public async Task<Result<ObjectId?>> GetFirstEnteredId()
    {
        _logger.LogInformation("Get first entered receive id.");
        var receive = _context.Receives.AsNoTracking().FirstOrDefaultAsync(x => x.Status == ReceiveStatus.Entered).Result;

        if (receive == null)
            return Result<ObjectId?>.Error("Unable to get first entered receive id.");

        return Result<ObjectId?>.Ok(receive.Id);
    }

    public async Task<Result<Receive?>> UpdateAsync(Receive receive)
    {
        _logger.LogInformation("Update receive: {Id}", receive.Id);
        _context.Receives.Update(receive);

        if (_context.SaveChanges() == 0)
            return Result<Receive?>.Error("Unable to update receive.");

        return Result<Receive?>.Ok(receive);
    }
}