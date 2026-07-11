using Pokedex.Api.Domain;

namespace Pokedex.Api.Infra.ApiClients.PokeApi;

/// <summary>Fetches Pokémon data from the PokéAPI.</summary>
public interface IPokemonApiClient
{
    /// <summary>Fetches a Pokémon by name (case-insensitive).</summary>
    /// <exception cref="Pokedex.Api.Exceptions.PokemonNotFoundException">The Pokémon does not exist (PokéAPI 404).</exception>
    /// <exception cref="HttpRequestException">The PokéAPI returned a non-success status (upstream failure).</exception>
    Task<Pokemon> GetPokemonAsync(string name, CancellationToken ct = default);
}
