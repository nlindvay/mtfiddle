namespace mtfiddle;

public record NewReceive(string Reference)
{
    public static implicit operator NewReceive(string json)
    {
        NewReceive entity = json.ToType<NewReceive>();
        return entity;
    }
}