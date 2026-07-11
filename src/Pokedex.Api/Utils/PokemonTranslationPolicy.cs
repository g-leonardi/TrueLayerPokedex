using Pokedex.Api.Domain;

namespace Pokedex.Api.Utils;

/// <summary>
/// The business rule that decides which translator applies to a Pokémon: Yoda if it lives
/// in a cave OR is legendary, Shakespeare otherwise. Pure and side-effect free.
/// </summary>
public static class PokemonTranslationPolicy
{
    private const string YodaHabitat = "cave";

    /// <summary>Chooses the translation for the given Pokémon.</summary>
    public static TranslationKind GetTranslationKind(Pokemon pokemon)
        => YodaHabitat.Equals(pokemon.Habitat) || pokemon.IsLegendary
            ? TranslationKind.Yoda
            : TranslationKind.Shakespeare;
}
