using Pokedex.Api.Domain;

namespace Pokedex.Api.Infra.ApiClients.Translation;

/// <summary>Translates text via the FunTranslations service.</summary>
public interface ITranslationApiClient
{
    /// <summary>
    /// Best-effort translation. Returns the translated text, or <c>null</c> on ANY failure
    /// (rate limit, 4xx/5xx, timeout, malformed response) so the caller can fall back to the
    /// standard description. It never throws because of a translation failure.
    /// </summary>
    Task<string?> TranslateAsync(string text, TranslationKind kind, CancellationToken ct = default);
}
