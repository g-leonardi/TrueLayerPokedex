using System.Text.Json.Serialization;

namespace Pokedex.Api.Infra.Contracts.Translation;

// Shape of the FunTranslations response — only `contents.translated` is read.
public record TranslationDTO(
    [property: JsonPropertyName("contents")]
    TranslationContentsDTO Contents
);

public record TranslationContentsDTO(
    [property: JsonPropertyName("translated")]
    string Translated
);
