using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Pokedex.Api.Domain;                       // TranslationKind
using Pokedex.Api.Infra.ApiClients.Translation; // TranslationApiClient, TranslationApiOptions

namespace Pokedex.Api.Tests;

// Best-effort contract of the translation client: on a 200 with a valid body it returns
// contents.translated; on ANY failure (429/5xx/malformed body/…) it returns null so the
// caller can fall back to the standard description. We simulate every case with a fake
// HttpMessageHandler — no network, deterministic.
public class TranslationApiClientTests
{
    // Fake handler: ignores the request and always returns the preconfigured response.
    private sealed class StubHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest {get; private set;}
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken ct){
                LastRequest = request;
                return Task.FromResult(response);}
    }

    // Builds a TranslationApiClient over the fake handler.
    // The client posts to ABSOLUTE URLs (from options), so no BaseAddress is needed here.
    private static TranslationApiClient ClientReturning(HttpResponseMessage response)
    {
        var http = new HttpClient(new StubHandler(response));
        var options = Options.Create(new TranslationApiOptions
        {
            YodaUrl = "https://test/yoda",
            ShakespeareUrl = "https://test/shakespeare"
        });
        return new TranslationApiClient(http, options, NullLogger<TranslationApiClient>.Instance);
    }

     private static TranslationApiClient ClientReturning(StubHandler stub)
    {
        var http = new HttpClient(stub);
        var options = Options.Create(new TranslationApiOptions
        {
            YodaUrl = "https://test/yoda",
            ShakespeareUrl = "https://test/shakespeare"
        });
        return new TranslationApiClient(http, options, NullLogger<TranslationApiClient>.Instance);
    }

    private static HttpResponseMessage JsonResponse(HttpStatusCode status, string json="") =>
        new(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") };

    // A valid FunTranslations success body (only contents.translated is read).
    private const string YodaJson = @"{
        ""success"": { ""total"": 1 },
        ""contents"": {
            ""translated"": ""Powerful you have become."",
            ""text"": ""You have become powerful."",
            ""translation"": ""yoda""
        }
    }";



    // The shape the mirror returns on rate limit (status is 429; body is this).
    private const string RateLimitJson = @"{
        ""error"": { ""code"": 429, ""message"": ""Too many requests."" },
        ""retry_after"": 42
    }";

    // 1) HAPPY PATH — 200 + valid body → returns contents.translated.  (worked example)
    [Fact]
    public async Task TranslateAsync_SuccessfulResponse_ReturnsTranslatedText()
    {
        var client = ClientReturning(JsonResponse(HttpStatusCode.OK, YodaJson));

        var result = await client.TranslateAsync("You have become powerful.", TranslationKind.Yoda);

        Assert.Equal("Powerful you have become.", result);
    }

    // 2) RATE LIMITED — 429 → best-effort must return null (→ caller falls back).
    [Fact]
    public async Task TranslateAsync_RateLimited_ReturnsNull()
    {
        var client = ClientReturning(JsonResponse(HttpStatusCode.TooManyRequests, RateLimitJson));

        var result = await client.TranslateAsync("test", TranslationKind.Yoda);
        Assert.Null(result);
    }

    // 3) UPSTREAM ERROR — 500 → return null (the mirror is broken, not us).
    [Fact]
    public async Task TranslateAsync_ServerError_ReturnsNull()
    {
        var client = ClientReturning(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var result = await client.TranslateAsync("test", TranslationKind.Yoda);

        Assert.Null(result);

    }

    // 4) GARBAGE BODY — 200 but the body is NOT the expected JSON (e.g. an HTML block page,
    //    exactly like the Cloudflare 403 I saw). Deserialization fails → must return null.
    [Fact]
    public async Task TranslateAsync_MalformedBody_ReturnsNull()
    {
        var client = ClientReturning(JsonResponse(HttpStatusCode.OK, "<html>not json</html>"));

        var result = await client.TranslateAsync("test", TranslationKind.Yoda);

        Assert.Null(result);
    }

    // ROUTING — kind=Shakespeare must POST to the Shakespeare URL (not the Yoda one).
    [Fact]
    public async Task TranslateAsync_ShakespeareKind_PostsToShakespeareUrl()
    {
        StubHandler handler = new StubHandler(JsonResponse(HttpStatusCode.OK));
        var client = ClientReturning(handler);

        // Body irrelevant here: we assert on the URL we SENT to, not on the result.
        await client.TranslateAsync("You have become powerful.", TranslationKind.Shakespeare);

        Assert.Equal("https://test/shakespeare", handler.LastRequest!.RequestUri!.ToString());
    }

    
}
