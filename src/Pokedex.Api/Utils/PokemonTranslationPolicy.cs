using Pokedex.Api.Domain;

namespace Pokedex.Api.Utils;

public static class PokemonTranslationPolicy
{
    private const string YodaHabitat ="cave";
  
    public static TranslationKind GetTranslationKind(Pokemon pkmn)
    {
        return YodaHabitat.Equals(pkmn.Habitat) || pkmn.IsLegendary ? TranslationKind.Yoda : TranslationKind.Shakespeare;   
    }
}