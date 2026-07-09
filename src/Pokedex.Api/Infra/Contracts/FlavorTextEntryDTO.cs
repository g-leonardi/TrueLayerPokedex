using System.Text.Json.Serialization;

namespace Pokedex.Api.Infra.Contracts;

public record FlavorTextEntryDTO
(
    [property: JsonPropertyName("flavor_text")]
    string FlavorText,

    [property: JsonPropertyName("language")]
    LanguageDTO Language

);
   
