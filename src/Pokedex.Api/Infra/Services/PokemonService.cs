using Pokedex.Api.Domain;
using Pokedex.Api.Infra.ApiClients.PokeApi;
using Pokedex.Api.Infra.ApiClients.Translation;
using Pokedex.Api.Utils;

namespace Pokedex.Api.Infra.Services;

/// <summary>
/// Application service behind the two endpoints. Fetches the Pokémon and, for the translated
/// endpoint, chooses a translator and applies a best-effort translation with graceful
/// fallback to the standard description.
/// </summary>
public class PokemonService : IPokemonService
{
    private readonly IPokemonApiClient _pokemonApi;
    private readonly ITranslationApiClient _translationApi;
    private readonly ILogger<PokemonService> _logger;

    public PokemonService(
        IPokemonApiClient pokemonApi,
        ITranslationApiClient translationApi,
        ILogger<PokemonService> logger)
    {
        _pokemonApi = pokemonApi;
        _translationApi = translationApi;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<Pokemon> GetPokemonAsync(string pokemonName, CancellationToken ct)
        => _pokemonApi.GetPokemonAsync(pokemonName, ct);

    /// <inheritdoc />
    public async Task<Pokemon> GetPokemonTranslatedAsync(string pokemonName, CancellationToken ct)
    {
        Pokemon pokemon = await _pokemonApi.GetPokemonAsync(pokemonName, ct);

        TranslationKind kind = PokemonTranslationPolicy.GetTranslationKind(pokemon);
        _logger.LogDebug("Selected {TranslationKind} translation for {PokemonName}", kind, pokemon.Name);

        string? translated = await _translationApi.TranslateAsync(pokemon.Description, kind, ct);

        if (string.IsNullOrEmpty(translated))
        {
            _logger.LogDebug("No translation available for {PokemonName}; keeping the standard description", pokemon.Name);
            return pokemon;
        }

        return pokemon with { Description = translated };
    }
}
