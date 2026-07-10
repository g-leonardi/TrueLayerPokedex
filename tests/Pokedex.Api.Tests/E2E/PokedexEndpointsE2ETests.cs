using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Pokedex.Api.Infra.ApiClients.PokeApi;
using Pokedex.Api.Infra.ApiClients.Translation;

namespace Pokedex.Api.Tests;

// End-to-end through the REAL HTTP stack (routing → DI → service → typed clients),
// with only the OUTBOUND network faked (a stub primary handler per typed client).
// This proves what unit tests can't: DI resolves, routes map, status codes and the
// JSON contract are correct — and it stays deterministic (no live PokéAPI / mirror).
public class PokedexEndpointsE2ETests
{
    private sealed class StubHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken ct)
            => Task.FromResult(response);
    }

    private static HttpResponseMessage Json(HttpStatusCode status, string body) =>
        new(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    // Minimal pokemon-species body the mapper reads (mewtwo: legendary, habitat "rare").
    private const string MewtwoSpecies = @"{
        ""name"": ""mewtwo"",
        ""is_legendary"": true,
        ""habitat"": { ""name"": ""rare"" },
        ""flavor_text_entries"": [
            { ""flavor_text"": ""It was created by a scientist."", ""language"": { ""name"": ""en"" } }
        ]
    }";

    private const string YodaTranslation =
        @"{ ""contents"": { ""translated"": ""Created by a scientist, it was."" } }";

    // Spins up the app with each typed client's network replaced by a canned response.
    private static HttpClient CreateClient(
        HttpResponseMessage pokeApiResponse,
        HttpResponseMessage translationResponse)
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                {
                    services.AddHttpClient<IPokemonApiClient, PokemonApiClient>()
                        .ConfigurePrimaryHttpMessageHandler(() => new StubHandler(pokeApiResponse));
                    services.AddHttpClient<ITranslationApiClient, TranslationApiClient>()
                        .ConfigurePrimaryHttpMessageHandler(() => new StubHandler(translationResponse));
                }));
        return factory.CreateClient();
    }

    // The public wire contract we assert against (camelCase JSON → these names).
    private sealed record PokemonResponse(string Name, string Description, string? Habitat, bool IsLegendary);

    // ENDPOINT 1 — basic info: 200 + the standard contract.
    [Fact]
    public async Task GetPokemon_ReturnsBasicInfo()
    {
        var client = CreateClient(
            Json(HttpStatusCode.OK, MewtwoSpecies),
            Json(HttpStatusCode.OK, YodaTranslation));

        var response = await client.GetAsync("/pokemon/mewtwo");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<PokemonResponse>();
        Assert.Equal("mewtwo", body!.Name);
        Assert.Equal("rare", body.Habitat);
        Assert.True(body.IsLegendary);
        Assert.Equal("It was created by a scientist.", body.Description);
    }

    // ENDPOINT 2 — translated: 200 + the TRANSLATED description.
    [Fact]
    public async Task GetTranslatedPokemon_ReturnsTranslatedDescription()
    {
        var client = CreateClient(
            Json(HttpStatusCode.OK, MewtwoSpecies),
            Json(HttpStatusCode.OK, YodaTranslation));

        var response = await client.GetAsync("/pokemon/translated/mewtwo");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<PokemonResponse>();
        Assert.Equal("Created by a scientist, it was.", body!.Description);
    }

    // ENDPOINT 2 — the headline behaviour: translation fails (429) → still 200 with the STANDARD description.
    [Fact]
    public async Task GetTranslatedPokemon_WhenTranslationFails_FallsBackToStandardDescription()
    {
        var client = CreateClient(
            Json(HttpStatusCode.OK, MewtwoSpecies),
            Json(HttpStatusCode.TooManyRequests, "{}"));   // translation upstream is down

        var response = await client.GetAsync("/pokemon/translated/mewtwo");

        response.EnsureSuccessStatusCode();   // still 200 — degraded gracefully
        var body = await response.Content.ReadFromJsonAsync<PokemonResponse>();
        Assert.Equal("It was created by a scientist.", body!.Description);   // standard, NOT translated
    }
}
