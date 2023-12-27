namespace mtfiddle;

public record NewEvent(string TargetType, string TargetId, string NewStatus)
{
    public static implicit operator NewEvent(string json)
    {
        NewEvent entity = json.ToType<NewEvent>();
        return entity;
    }
}