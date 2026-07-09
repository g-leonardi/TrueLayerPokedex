namespace Pokedex.Api.Domain;

public record Pokemon(
    string Name,
    string Description,
    string? Habitat,
    bool IsLegendary);

public enum TranslationKind
{
    None,
    Yoda,
    Shakespeare
}