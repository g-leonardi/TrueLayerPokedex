using Pokedex.Api.Domain;
using Pokedex.Api.Infra.Contracts;
using Pokedex.Api.Exceptions; 

namespace Pokedex.Api.Utils;

public static class PokemonMapper
{
    private const string EnglishLanguageCode="en";
  
    public static Pokemon ToDomain(PokemonSpeciesDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var englishDescription = dto.FlavorTextEntries.FirstOrDefault(x => EnglishLanguageCode.Equals(x.Language?.Name))?.FlavorText;


        if (string.IsNullOrWhiteSpace(englishDescription))
        {
            throw new NoPokemonDescriptionException(dto.Name);
        }

        var sanitizeDescription = DescriptionSanitizer.Sanitize(englishDescription);

        return new Pokemon
        (
            Name : dto.Name,
            Description : sanitizeDescription,
            IsLegendary : dto.IsLegendary,
            Habitat : dto.Habitat?.Name
        );
            
    }
}