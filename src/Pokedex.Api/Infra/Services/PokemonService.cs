using Pokedex.Api.Domain;
using Pokedex.Api.Infra.ApiClients;

namespace Pokedex.Api.Infra.Services;

public class PokemonService : IPokemonService
{
    private readonly IPokemonApiClient _pokemonApi;

    public PokemonService(IPokemonApiClient pokemonApi)
    {
        _pokemonApi = pokemonApi;
    }

    public async Task<Pokemon> GetPokemonAsync(string pokemonName, CancellationToken ct)
    {
        return await _pokemonApi.GetPokemonAsync(pokemonName,ct);
    }

   

    
}