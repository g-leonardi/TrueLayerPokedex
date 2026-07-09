using System.Text.Json.Serialization;

namespace Pokedex.Api.Infra.Contracts;

public record PokemonSpeciesDTO(
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("is_legendary")]
    bool IsLegendary,
    [property: JsonPropertyName("habitat")]
    HabitatDTO? Habitat,
    [property: JsonPropertyName("flavor_text_entries")]
    FlavorTextEntryDTO[] FlavorTextEntries
);




