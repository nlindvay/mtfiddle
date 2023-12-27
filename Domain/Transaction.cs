using System;
using MongoDB.Bson;

namespace mtfiddle;

public class Transaction
{
    public ObjectId Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public string Status { get; set; }
    public string Reference { get; set; }

    public static implicit operator Transaction(NewTransaction newTransaction) => new Transaction
    {
        Id = ObjectId.GenerateNewId(),
        CreatedOn = DateTime.UtcNow,
        Status = TransactionStatus.Unpaid,
        Reference = newTransaction.Reference
    };
    
    public void ApplyEventUpdates(Event @event)
    {
        UpdatedOn = @event.CreatedOn;
        Status = @event.NewStatus ?? Status;
    }
}