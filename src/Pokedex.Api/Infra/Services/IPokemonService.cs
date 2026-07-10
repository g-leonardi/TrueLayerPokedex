using Pokedex.Api.Domain;

namespace Pokedex.Api.Infra.Services
{
    public interface IPokemonService
    {
        Task<Pokemon> GetPokemonAsync(string pokemonName, CancellationToken ct);
        Task<Pokemon> GetPokemonTranslatedAsync(string name, CancellationToken ct);    }
    
    }