using System.Text.Json.Serialization;

namespace Pokedex.Api.Infra.Contracts;

public record LanguageDTO
(
    [property:JsonPropertyName("name")]
    string Name
);
   
