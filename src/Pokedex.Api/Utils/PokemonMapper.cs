using Pokedex.Api.Domain;
using Pokedex.Api.Infra.Contracts.Pokemon;
using Pokedex.Api.Exceptions;

namespace Pokedex.Api.Utils;

/// <summary>Maps the raw PokéAPI <c>pokemon-species</c> DTO onto the clean domain model.</summary>
public static class PokemonMapper
{
    private const string EnglishLanguageCode = "en";

    /// <summary>
    /// Builds a <see cref="Pokemon"/> from the species DTO: takes the first English flavor
    /// text as the description and sanitizes it; <c>habitat</c> may be null.
    /// </summary>
    /// <exception cref="NoPokemonDescriptionException">No English description is available.</exception>
    public static Pokemon ToDomain(PokemonSpeciesDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var englishDescription = dto.FlavorTextEntries
            .FirstOrDefault(x => EnglishLanguageCode.Equals(x.Language?.Name))?.FlavorText;

        if (string.IsNullOrWhiteSpace(englishDescription))
            throw new NoPokemonDescriptionException(dto.Name);

        var sanitizedDescription = DescriptionSanitizer.Sanitize(englishDescription);

        return new Pokemon(
            Name: dto.Name,
            Description: sanitizedDescription,
            Habitat: dto.Habitat?.Name,
            IsLegendary: dto.IsLegendary);
    }
}
