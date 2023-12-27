namespace mtfiddle;

public record ClaimCheckType
{
    private const string Application = "mtfiddle";
    private const string Order = "order";
    private const string Receive = "receive";
    private const string Transaction = "transaction";
    private const string Event = "event";

    public string OrderType => $"{Application}.{Order}";
    public string ReceiveType => $"{Application}.{Receive}";
    public string TransactionType => $"{Application}.{Transaction}";
    public string EventType => $"{Application}.{Event}";

}