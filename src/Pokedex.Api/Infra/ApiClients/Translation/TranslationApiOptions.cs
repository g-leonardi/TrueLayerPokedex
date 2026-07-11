namespace Pokedex.Api.Infra.ApiClients.Translation;

/// <summary>
/// Configuration for the FunTranslations client, bound from the <c>TranslationApi</c> section.
/// It's a separate external service from the PokéAPI, hence its own options and config section.
/// </summary>
public class TranslationApiOptions
{
    public const string SectionName = "TranslationApi";

    /// <summary>Full URL of the Yoda endpoint, e.g. <c>https://api.funtranslations.mercxry.me/v1/translate/yoda</c>.</summary>
    public string YodaUrl { get; set; } = "";

    /// <summary>Full URL of the Shakespeare endpoint.</summary>
    public string ShakespeareUrl { get; set; } = "";
}
