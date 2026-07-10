using System.Net;
using System.Text.Json;
using Pokedex.Api.Domain;
using Pokedex.Api.Exceptions;
using Pokedex.Api.Infra.Contracts.Pokemon;
using Pokedex.Api.Utils;

namespace Pokedex.Api.Infra.ApiClients;

public class PokemonApiClient : IPokemonApiClient
{
    private readonly HttpClient _http;
    
    public PokemonApiClient(HttpClient http) => _http = http;

    public async Task<Pokemon> GetPokemonAsync(string name, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"pokemon-species/{name.ToLowerInvariant().Trim()}", ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new PokemonNotFoundException(name);

        response.EnsureSuccessStatusCode();
        
        var species = await response.Content.ReadFromJsonAsync<PokemonSpeciesDTO>(ct);
        return species != null ? PokemonMapper.ToDomain(species) : throw new InvalidOperationException($"Failed to deserialize Pokemon species response for '{name}'.");
    }
}
