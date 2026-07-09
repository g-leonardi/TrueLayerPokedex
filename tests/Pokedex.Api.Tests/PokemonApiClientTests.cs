using System.Net;
using System.Text;
using Pokedex.Api.Exceptions;
using Pokedex.Api.Infra.ApiClients;

namespace Pokedex.Api.Tests;

// ─────────────────────────────────────────────────────────────────────────────
// How to test an HTTP client WITHOUT a network:
//   HttpClient doesn't talk to sockets directly: it delegates to an HttpMessageHandler.
//   We write a FAKE one (StubHandler) that, instead of making real network calls, returns
//   a response we decide. This way tests are deterministic and fast
// ─────────────────────────────────────────────────────────────────────────────

public class PokemonApiClientTests
{
    // Fake handler: ignores the request and always returns the preconfigured response.
    private sealed class StubHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken ct)
            => Task.FromResult(response);
    }

    // Creates a PokemonApiClient on an HttpClient that uses the fake handler.
    // The dummy BaseAddress is mandatory: the client calls relative URLs.
    private static PokemonApiClient ClientReturning(HttpResponseMessage response)
    {
        var http = new HttpClient(new StubHandler(response))
        {
            BaseAddress = new Uri("https://test/")
        };
        return new PokemonApiClient(http);
    }

    private static HttpResponseMessage JsonResponse(HttpStatusCode status, string json) =>
        new(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") };

    // Sample JSON, reduced to only the fields the mapper reads.
    // Note the \n inside flavor_text: it verifies that sanitization
    // passes through end-to-end (client → deserialization → mapper).
    private const string MewtwoJson = @"{
        ""name"": ""mewtwo"",
        ""is_legendary"": true,
        ""habitat"": { ""name"": ""rare"" },
        ""flavor_text_entries"": [
            { ""flavor_text"": ""It was created by\na scientist."", ""language"": { ""name"": ""en"" } }
        ]
    }";

    // 1) HAPPY PATH — 200 + valid JSON → Pokémon mapped correctly.
    [Fact]
    public async Task GetPokemonAsync_SuccessfulResponse_ReturnsMappedPokemon()
    {
        var client = ClientReturning(JsonResponse(HttpStatusCode.OK, MewtwoJson));

        var result = await client.GetPokemonAsync("mewtwo");

        Assert.Equal("mewtwo", result.Name);
        Assert.True(result.IsLegendary);
        Assert.Equal("rare", result.Habitat);
        Assert.Equal("It was created by a scientist.", result.Description); // \n → space
        Assert.DoesNotContain("\n", result.Description);
    }

    // 2) NOT FOUND — 404 → the domain-specific exception (not a generic error).
    [Fact]
    public async Task GetPokemonAsync_NotFound_ThrowsPokemonNotFoundException()
    {
        var client = ClientReturning(new HttpResponseMessage(HttpStatusCode.NotFound));

        var ex = await Assert.ThrowsAsync<PokemonNotFoundException>(
            () => client.GetPokemonAsync("missingno"));
        Assert.Contains("missingno", ex.Message); 
    }

    // 3) UPSTREAM DOWN — 500 → must PROPAGATE HttpRequestException, NOT become 404.
    [Fact]
    public async Task GetPokemonAsync_ServerError_PropagatesHttpRequestException()
    {
        var client = ClientReturning(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        await Assert.ThrowsAsync<HttpRequestException>(
            () => client.GetPokemonAsync("mewtwo"));
    }

    // 4) NULL BODY — 200 but the body deserializes to null → InvalidOperationException.
    [Fact]
    public async Task GetPokemonAsync_NullBody_ThrowsInvalidOperationException()
    {
        var client = ClientReturning(JsonResponse(HttpStatusCode.OK, "null"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => client.GetPokemonAsync("mewtwo"));
    }
}
