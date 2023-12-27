using System;
using MongoDB.Bson;

namespace mtfiddle;

public class Event
{
    public ObjectId Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public string TargetType { get; set; }
    public string TargetId { get; set; }
    public string NewStatus { get; set; }

    public static implicit operator Event(NewEvent newEvent) => new Event
    {
        Id = ObjectId.Parse(newEvent.TargetId),
        CreatedOn = DateTime.UtcNow,
        TargetType = newEvent.TargetType,
        TargetId = newEvent.TargetId,
        NewStatus = newEvent.NewStatus
    };
}