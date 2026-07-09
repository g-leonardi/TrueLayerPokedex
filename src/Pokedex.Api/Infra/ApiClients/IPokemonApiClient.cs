using Pokedex.Api.Domain;

namespace Pokedex.Api.Infra.ApiClients
{
    public interface IPokemonApiClient
    {
        Task<Pokemon> GetPokemonAsync(string name, CancellationToken ct = default);
    }
}