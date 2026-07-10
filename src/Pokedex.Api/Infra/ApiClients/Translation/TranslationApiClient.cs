using Microsoft.Extensions.Options;
using Pokedex.Api.Domain;
using Pokedex.Api.Exceptions;
using Pokedex.Api.Infra.Contracts.Translation;

namespace Pokedex.Api.Infra.ApiClients.Translation;

public class TranslationApiClient : ITranslationApiClient
{
    private readonly HttpClient _http;
    private readonly TranslationApiOptions _options;

    public TranslationApiClient(HttpClient http, IOptions<TranslationApiOptions> options)
    {
        _http = http;
        _options = options.Value;
    }

    public async Task<string?> TranslateAsync(string text, TranslationKind kind, CancellationToken ct = default)
    {
        var url = ResolveUrl(kind);
        try
        {
            var response = await _http.PostAsJsonAsync(url, new { text }, ct);
            response.EnsureSuccessStatusCode();
            var translation = await response.Content.ReadFromJsonAsync<TranslationDTO>(ct);
            return translation?.Contents?.Translated;


        }catch(Exception)
        {
            return null;
        }
    }

    private string ResolveUrl(TranslationKind kind) => kind switch
    {
        TranslationKind.Yoda => _options.YodaUrl,
        TranslationKind.Shakespeare => _options.ShakespeareUrl,
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unsupported translation kind.")
    };
}
