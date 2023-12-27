namespace mtfiddle;

public class TypeOptions
{
    public const string Application = "mtfiddle";
    public const string Order = "order";
    public const string Receive = "receive";
    public const string Product = "product";
    public const string Transaction = "transaction";
    public const string Event = "event";

    public string OrderType => $"{Application}.{Order}";
    public string ReceiveType => $"{Application}.{Receive}";
    public string ProductType => $"{Application}.{Product}";
    public string TransactionType => $"{Application}.{Transaction}";
    public string EventType => $"{Application}.{Event}";

}