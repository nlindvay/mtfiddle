namespace mtfiddle;

public record NewOrder(string Reference)
{
    public static implicit operator NewOrder(string json)
    {
        NewOrder entity = json.ToType<NewOrder>();
        return entity;
    }
}