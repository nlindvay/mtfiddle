namespace mtfiddle;

public record Result<T>(int Status, string Message, T? Value)
{
    public static Result<T?> Ok(T? value) => new(200, "OK", value);
    public static Result<T?> Error(string message) => new(500, message, default);
    public static Result<T?> NotFound(string message) => new(404, message, default);
    public bool IsSuccessful => Status == 200;
}