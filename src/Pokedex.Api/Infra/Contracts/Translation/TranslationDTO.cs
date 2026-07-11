using System.Text.Json.Serialization;

namespace Pokedex.Api.Infra.Contracts.Translation;

/// <summary>Shape of the FunTranslations response — only <c>contents.translated</c> is read.</summary>
public record TranslationDTO(
    [property: JsonPropertyName("contents")]
    TranslationContentsDTO Contents
);

public record TranslationContentsDTO(
    [property: JsonPropertyName("translated")]
    string Translated
);
