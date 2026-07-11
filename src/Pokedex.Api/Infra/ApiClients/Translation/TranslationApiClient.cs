using Microsoft.Extensions.Options;
using Pokedex.Api.Domain;
using Pokedex.Api.Infra.Contracts.Translation;

namespace Pokedex.Api.Infra.ApiClients.Translation;

/// <summary>
/// Typed <see cref="HttpClient"/> for the FunTranslations API. Best-effort by design: any
/// failure becomes a <c>null</c> result, letting the caller degrade gracefully to the
/// standard description.
/// </summary>
public class TranslationApiClient : ITranslationApiClient
{
    private readonly HttpClient _http;
    private readonly TranslationApiOptions _options;
    private readonly ILogger<TranslationApiClient> _logger;

    public TranslationApiClient(
        HttpClient http,
        IOptions<TranslationApiOptions> options,
        ILogger<TranslationApiClient> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string?> TranslateAsync(string text, TranslationKind kind, CancellationToken ct = default)
    {
        var url = ResolveUrl(kind);
        _logger.LogDebug("Requesting {TranslationKind} translation", kind);

        try
        {
            var response = await _http.PostAsJsonAsync(url, new { text }, ct);
            response.EnsureSuccessStatusCode();
            var translation = await response.Content.ReadFromJsonAsync<TranslationDTO>(ct);
            return translation?.Contents?.Translated;
        }
        catch (Exception ex)
        {
            // Any failure (rate limit / network / bad body) is non-fatal: log and degrade.
            _logger.LogWarning(ex, "Translation failed for {TranslationKind}; using the standard description", kind);
            return null;
        }
    }

    // Maps the abstract choice to the concrete FunTranslations endpoint (implementation detail).
    private string ResolveUrl(TranslationKind kind) => kind switch
    {
        TranslationKind.Yoda => _options.YodaUrl,
        TranslationKind.Shakespeare => _options.ShakespeareUrl,
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unsupported translation kind.")
    };
}
