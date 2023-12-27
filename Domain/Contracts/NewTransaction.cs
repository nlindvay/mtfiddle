namespace mtfiddle;

public record NewTransaction(string Reference)
{
    public static implicit operator NewTransaction(string json)
    {
        NewTransaction entity = json.ToType<NewTransaction>();
        return entity;
    }
}