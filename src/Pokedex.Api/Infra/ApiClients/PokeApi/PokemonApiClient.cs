using System.Net;
using Pokedex.Api.Domain;
using Pokedex.Api.Exceptions;
using Pokedex.Api.Infra.Contracts.Pokemon;
using Pokedex.Api.Utils;

namespace Pokedex.Api.Infra.ApiClients.PokeApi;

/// <summary>Typed <see cref="HttpClient"/> for the PokéAPI <c>pokemon-species</c> resource.</summary>
public class PokemonApiClient : IPokemonApiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<PokemonApiClient> _logger;

    public PokemonApiClient(HttpClient http, ILogger<PokemonApiClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Pokemon> GetPokemonAsync(string name, CancellationToken ct = default)
    {
        // PokéAPI only accepts lowercase, unpadded names in the path.
        var normalized = name.ToLowerInvariant().Trim();
        _logger.LogDebug("Requesting species for {PokemonName} from PokéAPI", normalized);

        var response = await _http.GetAsync($"pokemon-species/{normalized}", ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new PokemonNotFoundException(name);

        response.EnsureSuccessStatusCode();

        var species = await response.Content.ReadFromJsonAsync<PokemonSpeciesDTO>(ct);
        return species != null
            ? PokemonMapper.ToDomain(species)
            : throw new InvalidOperationException($"Failed to deserialize Pokemon species response for '{name}'.");
    }
}
