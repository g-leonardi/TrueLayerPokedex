namespace Pokedex.Api.Infra.ApiClients;

// Config for the FunTranslations service — a DIFFERENT external service than the
// PokéAPI, so it lives in its own options class bound to its own config section.
public class TranslationApiOptions
{
    public const string SectionName = "TranslationApi";
    public string YodaUrl { get; set; } = "";        // e.g. "https://api.funtranslations.com/translate/yoda.json"
    public string ShakespeareUrl { get; set; } = ""; // e.g. "https://api.funtranslations.com/translate/shakespeare.json"
}
