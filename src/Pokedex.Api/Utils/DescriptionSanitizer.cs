using System.Text.RegularExpressions;

namespace Pokedex.Api.Utils;

/// <summary>
/// Cleans a PokéAPI flavor text: control characters (newlines, form feeds) and any repeated
/// whitespace are collapsed into single spaces, so the description reads as one clean line.
/// </summary>
public static class DescriptionSanitizer
{
    /// <summary>Collapses all whitespace runs to single spaces and trims the result.</summary>
    public static string Sanitize(string description)
    {
        if (string.IsNullOrEmpty(description))
            return description;

        var sanitized = Regex.Replace(description, @"\s+", " ");
        return sanitized.Trim();
    }
}
