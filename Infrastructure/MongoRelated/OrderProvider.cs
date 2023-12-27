using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using mtfiddle.Application;

namespace mtfiddle;

public class OrderProvider : IOrderProvider
{
    private readonly AppDbContext _context;
    private readonly ILogger<OrderProvider> _logger;

    public OrderProvider(AppDbContext context, ILogger<OrderProvider> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Order?>> CreateAsync(Order order)
    {
        _logger.LogInformation("Create order: {Id}", order.Id);
        _context.Orders.Add(order);

        if (_context.SaveChanges() == 0)
            return Result<Order?>.Error("Unable to create order.");

        return Result<Order>.Ok(order);
    }

    public async Task<Result<Order?>> GetAsync(string id)
    {
        _logger.LogInformation("Get order: {Id}", id);
        var order = _context.Orders.FirstOrDefaultAsync(x => x.Id == ObjectId.Parse(id)).Result;

        if (order == null)
            return Result<Order?>.Error("Unable to get order.");

        return Result<Order?>.Ok(order);
    }

    public async Task<Result<Order?>> UpdateAsync(Order order)
    {
        _logger.LogInformation("Update order: {Id}", order.Id);
        _context.Orders.Update(order);

        if (_context.SaveChanges() == 0)
            return Result<Order?>.Error("Unable to update order.");

        return Result<Order>.Ok(order);
    }

    public async Task<Result<ObjectId?>> GetFirstEnteredId()
    {
        _logger.LogInformation("Get first entered order id.");
        var order = _context.Orders.FirstOrDefaultAsync(x => x.Status == OrderStatus.Entered).Result;

        if (order == null)
            return Result<ObjectId?>.Error("Unable to get first entered order id.");

        return Result<ObjectId?>.Ok(order.Id);
    }
}