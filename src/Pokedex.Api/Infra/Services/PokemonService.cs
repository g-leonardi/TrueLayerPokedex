using Pokedex.Api.Domain;
using Pokedex.Api.Infra.ApiClients.PokeApi;
using Pokedex.Api.Infra.ApiClients.Translation;
using Pokedex.Api.Utils;

namespace Pokedex.Api.Infra.Services;

public class PokemonService : IPokemonService
{
    private readonly IPokemonApiClient _pokemonApi;
    private readonly ITranslationApiClient _translationApi;

    public PokemonService(IPokemonApiClient pokemonApi, ITranslationApiClient translationApi)
    {
        _pokemonApi = pokemonApi;
        _translationApi = translationApi;
    }

    public async Task<Pokemon> GetPokemonAsync(string pokemonName, CancellationToken ct)
    {
        return await _pokemonApi.GetPokemonAsync(pokemonName, ct);
    }

    public async Task<Pokemon> GetPokemonTranslatedAsync(string pokemonName, CancellationToken ct)
    {
        Pokemon pkmn = await _pokemonApi.GetPokemonAsync(pokemonName, ct);
        TranslationKind kind = PokemonTranslationPolicy.GetTranslationKind(pkmn);
        string? translated = await _translationApi.TranslateAsync(pkmn.Description, kind, ct);
        return string.IsNullOrEmpty(translated) ? pkmn : pkmn with { Description = translated };
    }
}
