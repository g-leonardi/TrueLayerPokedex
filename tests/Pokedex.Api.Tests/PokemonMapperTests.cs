using Pokedex.Api.Domain;
using Pokedex.Api.Exceptions;
using Pokedex.Api.Infra.Contracts;
using Pokedex.Api.Utils;

namespace Pokedex.Api.Tests;



public class PokemonMapperTests
{
    private static FlavorTextEntryDTO Entry(string text, string language) =>
        new(text, new LanguageDTO(language));

    private static PokemonSpeciesDTO Species(
        FlavorTextEntryDTO[] entries,
        string name = "mewtwo",
        bool isLegendary = true,
        string? habitat = "rare") =>
        new(
            name,
            isLegendary,
            habitat is null ? null : new HabitatDTO(habitat),
            entries);

    // 1) HAPPY PATH
    [Fact]
    public void ToDomain_ValidSpecies_MapsCoreFields()
    {
        var dto = Species(entries: new[] { Entry("It is a legendary Pokemon.", "en") });

        var result = PokemonMapper.ToDomain(dto);

        Assert.Equal("mewtwo", result.Name);                         
        Assert.Equal("It is a legendary Pokemon.", result.Description);
        Assert.True(result.IsLegendary);
        Assert.Equal("rare", result.Habitat);
    }

    // 2) SANITIZE
    [Fact]
    public void ToDomain_DescriptionWithControlChars_IsSanitized()
    {
        var raw = "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.";
        var dto = Species(new[] { Entry(raw, "en") });

        var result = PokemonMapper.ToDomain(dto);

        Assert.DoesNotContain("\n", result.Description);
        Assert.DoesNotContain("\f", result.Description);
        Assert.DoesNotContain("  ", result.Description); // niente spazi doppi
        Assert.Equal(
            "It was created by a scientist after years of horrific gene splicing and DNA engineering experiments.",
            result.Description);
    }

    // 3) RIGHT LANGUAGE
    [Fact]
    public void ToDomain_MultipleLanguages_PicksFirstEnglishDeterministically()
    {
        // Arrange
        var dto = Species(new[]
        {
            Entry("descrizione italiana", "it"),
            Entry("FIRST english",        "en"),
            Entry("second english",       "en"),
        });

        var result = PokemonMapper.ToDomain(dto);

        Assert.Equal("FIRST english", result.Description);
    }

    // 4) HABITAT NULL 
    [Fact]
    public void ToDomain_NullHabitat_MapsToNullAndDoesNotThrow()
    {
        var dto = Species(new[] { Entry("desc", "en") }, habitat: null);

        var result = PokemonMapper.ToDomain(dto);

        Assert.Null(result.Habitat);
    }

    // 5) NO ENGLISH ENTRY
    [Fact]
    public void ToDomain_NoEnglishEntry_ThrowsNoPokemonDescriptionException()
    {
        var dto = Species(new[]
        {
            Entry("descrizione italiana", "it"),
            Entry("description francaise", "fr"),
        });

        var ex = Assert.Throws<NoPokemonDescriptionException>(() => PokemonMapper.ToDomain(dto));
        Assert.Contains("mewtwo", ex.Message);
    }

  
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\n\f")]
    public void ToDomain_EnglishEntryButBlankText_Throws(string blank)
    {
        var dto = Species(new[] { Entry(blank, "en") });

        Assert.Throws<NoPokemonDescriptionException>(() => PokemonMapper.ToDomain(dto));
    }

    // 7) NULL CHECK
    [Fact]
    public void ToDomain_NullDto_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => PokemonMapper.ToDomain(null!));
    }
}
