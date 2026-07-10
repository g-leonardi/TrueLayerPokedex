using Pokedex.Api.Domain;

namespace Pokedex.Api.Infra.ApiClients.PokeApi;

public interface IPokemonApiClient
{
    Task<Pokemon> GetPokemonAsync(string name, CancellationToken ct = default);
}
