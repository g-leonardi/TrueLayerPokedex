using Pokedex.Api.Domain;
using Pokedex.Api.Infra.Contracts;
using Pokedex.Api.Exceptions;
using System.Reflection.Metadata.Ecma335;

namespace Pokedex.Api.Utils;

public static class PokemonTranslationPolicy
{
    private const string YodaHabitat ="cave";
  
    public static TranslationKind GetTranslationKind(Pokemon pkmn)
    {
        return YodaHabitat.Equals(pkmn.Habitat) || pkmn.IsLegendary ? TranslationKind.Yoda : TranslationKind.Shakespeare;   
    }
}