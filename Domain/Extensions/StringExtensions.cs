using System.Text.Json;

namespace mtfiddle;

public static class StringExtensions
{
    public static T ToType<T>(this string value)
    {
        return JsonSerializer.Deserialize<T>(value)!;
    }
}