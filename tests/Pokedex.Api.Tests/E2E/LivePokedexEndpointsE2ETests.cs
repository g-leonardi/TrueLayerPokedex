using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Pokedex.Api.Tests;

// LIVE end-to-end: hits the REAL PokéAPI and the REAL FunTranslations mirror — nothing faked.
// Non-deterministic by nature (network + the mirror's 5-req/min rate limit), so it is tagged
// [Trait("Category","Live")] and kept OUT of the default run. Toggle it in a pipeline with:
//     dotnet test --filter "Category!=Live"    → deterministic suite (CI default)
//     dotnet test --filter "Category=Live"     → these smoke tests (on demand)
//
// Their job is the one thing stubbed tests can't do: catch a drift between our assumptions
// and the real APIs' shape. So they assert INVARIANTS (status, shape) — NOT the volatile
// translated text, which may or may not come back depending on the rate limit.
[Trait("Category", "Live")]
public class LivePokedexEndpointsE2ETests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    private sealed record PokemonResponse(string Name, string Description, string? Habitat, bool IsLegendary);

    // ENDPOINT 1 — against the real PokéAPI.
    [Fact]
    public async Task GetPokemon_HitsRealPokeApi_ReturnsBasicInfo()
    {
        var response = await _client.GetAsync("/pokemon/mewtwo");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<PokemonResponse>();
        Assert.Equal("mewtwo", body!.Name);
        Assert.Equal("rare", body.Habitat);
        Assert.True(body.IsLegendary);
        Assert.False(string.IsNullOrWhiteSpace(body.Description));
    }

    // ENDPOINT 2 — against the real PokéAPI + real mirror.
    [Fact]
    public async Task GetTranslatedPokemon_HitsRealServices_ReturnsPokemon()
    {
        var response = await _client.GetAsync("/pokemon/translated/mewtwo");

        // 200 whether the translation SUCCEEDED (Yoda text) or the mirror rate-limited us
        // (fallback to the standard description) — both are valid, correct outcomes.
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<PokemonResponse>();
        Assert.Equal("mewtwo", body!.Name);
        Assert.Equal("rare", body.Habitat);
        Assert.True(body.IsLegendary);
        Assert.False(string.IsNullOrWhiteSpace(body.Description)); // present, but text NOT asserted
    }
}
