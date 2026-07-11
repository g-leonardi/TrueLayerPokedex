using System.Text.Json.Serialization;

namespace Pokedex.Api.Infra.Contracts.Pokemon;

/// <summary>
/// Shape of the PokéAPI <c>pokemon-species</c> response — only the fields we read. The nested
/// records have no life of their own (they're sub-parts of this one contract), so they live
/// together in a single file rather than one tiny file each.
/// </summary>
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

public record HabitatDTO(
    [property: JsonPropertyName("name")]
    string Name
);

public record FlavorTextEntryDTO(
    [property: JsonPropertyName("flavor_text")]
    string FlavorText,
    [property: JsonPropertyName("language")]
    LanguageDTO Language
);

public record LanguageDTO(
    [property: JsonPropertyName("name")]
    string Name
);
