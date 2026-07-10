using Pokedex.Api.Domain;

namespace Pokedex.Api.Infra.ApiClients.Translation;

public interface ITranslationApiClient
{
    // Best-effort: returns the translated text, or null on ANY failure (429/4xx/timeout/…)
    // so the caller can fall back to the standard description.
    Task<string?> TranslateAsync(string text, TranslationKind kind, CancellationToken ct = default);
}
