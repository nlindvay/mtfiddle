using System;
using MongoDB.Bson;

namespace mtfiddle;

public class Receive
{
    public ObjectId Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public string Status { get; set; }
    public string Reference { get; set; }

    public static implicit operator Receive(NewReceive newReceive) => new Receive
    {
        Id = ObjectId.GenerateNewId(),
        CreatedOn = DateTime.UtcNow,
        Status = ReceiveStatus.Entered,
        Reference = newReceive.Reference
    };

    public void ApplyEventUpdates(Event @event)
    {
        UpdatedOn = @event.CreatedOn;
        Status = @event.NewStatus ?? Status;
    }
}