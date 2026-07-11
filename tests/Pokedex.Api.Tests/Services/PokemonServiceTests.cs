using Microsoft.Extensions.Logging.Abstractions;
using Pokedex.Api.Domain;
using Pokedex.Api.Infra.ApiClients.PokeApi;
using Pokedex.Api.Infra.ApiClients.Translation;
using Pokedex.Api.Infra.Services;

namespace Pokedex.Api.Tests;

// The service's job on /translated: fetch the Pokémon, pick the translator, ask the
// translation client — and if it can't translate (null), fall back to the STANDARD
// description. Here we don't fake HTTP: we hand the service FAKE clients (whole
// interfaces) so we control exactly what each dependency returns. This is the seam
// that lets us test the fallback deterministically, without any network.
public class PokemonServiceTests
{
    // Fake PokéAPI client: always returns the Pokémon we hand it.
    private sealed class FakePokemonApiClient(Pokemon pokemon) : IPokemonApiClient
    {
        public Task<Pokemon> GetPokemonAsync(string name, CancellationToken ct = default)
            => Task.FromResult(pokemon);
    }

    // Fake translation client: returns the preset translation, or null to simulate failure.
    private sealed class FakeTranslationApiClient(string? translation) : ITranslationApiClient
    {
        public Task<string?> TranslateAsync(string text, TranslationKind kind, CancellationToken ct = default)
            => Task.FromResult(translation);
    }

    private static readonly Pokemon SamplePokemon =
        new("mewtwo", "It was created by a scientist.", "rare", IsLegendary: true);

    // A) TRANSLATION SUCCEEDS → the response carries the TRANSLATED description.  (worked example)
    [Fact]
    public async Task GetPokemonTranslatedAsync_TranslationSucceeds_UsesTranslatedDescription()
    {
        var service = new PokemonService(
            new FakePokemonApiClient(SamplePokemon),
            new FakeTranslationApiClient("Created by a scientist, it was."),
            NullLogger<PokemonService>.Instance);

        var result = await service.GetPokemonTranslatedAsync("mewtwo", default);

        Assert.Equal("Created by a scientist, it was.", result.Description);
    }

    // B) TRANSLATION FAILS (client returns null) → FALL BACK to the standard description.
    //    This is THE behaviour the challenge cares about most (the 😉 / rate limit).
    [Fact]
    public async Task GetPokemonTranslatedAsync_TranslationFails_FallsBackToStandardDescription()
    {
        var service = new PokemonService(
            new FakePokemonApiClient(SamplePokemon),
            new FakeTranslationApiClient(null),   // null = translation unavailable
            NullLogger<PokemonService>.Instance);

        var result = await service.GetPokemonTranslatedAsync("mewtwo", default);

        Assert.Equal("It was created by a scientist.", result.Description);
    }
}
