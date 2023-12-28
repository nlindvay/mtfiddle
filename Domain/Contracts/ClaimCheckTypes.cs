namespace mtfiddle;

public static class ClaimCheckType
{
    private const string Application = "mtfiddle";
    private const string Order = "order";
    private const string Receive = "receive";
    private const string Transaction = "transaction";
    private const string Event = "event";

    public static string OrderType => $"{Application}.{Order}";
    public static string ReceiveType => $"{Application}.{Receive}";
    public static string TransactionType => $"{Application}.{Transaction}";
    public static string EventType => $"{Application}.{Event}";

}