using System;
using MongoDB.Bson;

namespace mtfiddle;

public class Order
{
    public ObjectId Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public string Status { get; set; }
    public string Reference { get; set; }

    public static implicit operator Order(NewOrder newOrder) => new Order
    {
        Id = ObjectId.GenerateNewId(),
        CreatedOn = DateTime.UtcNow,
        Status = OrderStatus.Entered,
        Reference = newOrder.Reference
    };

    public void ApplyEventUpdates(Event @event)
    {
        UpdatedOn = @event.CreatedOn;
        Status = @event.NewStatus ?? Status;
    }
}