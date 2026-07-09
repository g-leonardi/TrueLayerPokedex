using System.Text.RegularExpressions;

namespace Pokedex.Api.Utils;

public static class DescriptionSanitizer
{
    public static string Sanitize(string description)
    {
        if (string.IsNullOrEmpty(description))
            return description;

        var sanitized = Regex.Replace(description, @"\s+", " ");
        return sanitized.Trim();
    }
}