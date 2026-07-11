namespace Pokedex.Api.Domain;

/// <summary>
/// A Pokémon as this API models it — the subset of PokéAPI data we care about, mapped onto
/// a clean domain shape. It also doubles as the public JSON response contract.
/// </summary>
/// <param name="Name">The Pokémon's name (lowercase).</param>
/// <param name="Description">The standard English description (sanitized); replaced by a
/// translated one on the <c>/pokemon/translated</c> endpoint.</param>
/// <param name="Habitat">The habitat name (e.g. "cave", "rare"); <c>null</c> when unknown.</param>
/// <param name="IsLegendary">Whether the Pokémon is legendary.</param>
public record Pokemon(
    string Name,
    string Description,
    string? Habitat,
    bool IsLegendary);

/// <summary>Which "fun translation" to apply to a Pokémon description.</summary>
public enum TranslationKind
{
    /// <summary>No translation. Unused in the current flow (every Pokémon gets Yoda or Shakespeare).</summary>
    None,

    /// <summary>Yoda-speak — for cave-dwelling or legendary Pokémon.</summary>
    Yoda,

    /// <summary>Shakespearean English — for everything else.</summary>
    Shakespeare
}
