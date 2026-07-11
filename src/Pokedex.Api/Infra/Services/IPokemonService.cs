using Pokedex.Api.Domain;

namespace Pokedex.Api.Infra.Services;

/// <summary>Orchestrates fetching a Pokémon and, optionally, translating its description.</summary>
public interface IPokemonService
{
    /// <summary>Returns a Pokémon with its standard description.</summary>
    Task<Pokemon> GetPokemonAsync(string pokemonName, CancellationToken ct);

    /// <summary>
    /// Returns a Pokémon whose description is translated (Yoda or Shakespeare, per
    /// <see cref="Pokedex.Api.Utils.PokemonTranslationPolicy"/>). Falls back to the standard
    /// description when the translation is unavailable.
    /// </summary>
    Task<Pokemon> GetPokemonTranslatedAsync(string pokemonName, CancellationToken ct);
}
