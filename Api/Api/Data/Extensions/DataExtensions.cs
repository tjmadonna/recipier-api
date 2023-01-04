using System.Text;

namespace Api.Data.Extensions;

public static class DataExtensions
{
    public static string ToSnakeCase(this string text)
    {
        if (text.Length < 2)
            return text.ToLowerInvariant();

        var sb = new StringBuilder();
        sb.Append(char.ToLowerInvariant(text[0]));
        for (int i = 1; i < text.Length; i++)
        {
            var c = text[i];
            if (char.IsUpper(c))
                sb.Append($"_{char.ToLowerInvariant(c)}");
            else
                sb.Append(c);
        }

        return sb.ToString();
    }
}