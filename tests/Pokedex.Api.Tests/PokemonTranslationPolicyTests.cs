using Pokedex.Api.Domain;
using Pokedex.Api.Utils;

namespace Pokedex.Api.Tests;

// The rule (§ challenge): Yoda when the habitat is "cave" OR the Pokémon is legendary;
// Shakespeare in every other case. The two conditions live on DIFFERENT fields
// (Habitat is a string, IsLegendary is a bool) — the row that catches the classic
// bug is the legendary Pokémon in a NON-cave habitat (mewtwo → "rare" → must be Yoda).
public class PokemonTranslationPolicyTests
{
    [Theory]
    [InlineData("cave", false, TranslationKind.Yoda)]        // cave habitat → Yoda
    [InlineData("rare", true, TranslationKind.Yoda)]         // legendary, non-cave habitat → Yoda (the trap)
    [InlineData("cave", true, TranslationKind.Yoda)]         // both conditions → Yoda
    [InlineData(null, true, TranslationKind.Yoda)]           // legendary with no habitat → Yoda
    [InlineData("grassland", false, TranslationKind.Shakespeare)] // ordinary + not legendary → Shakespeare
    [InlineData(null, false, TranslationKind.Shakespeare)]   // null habitat must not throw → Shakespeare
    public void GetTranslationKind_ChoosesTranslatorByHabitatAndLegendaryStatus(
        string? habitat, bool isLegendary, TranslationKind expected)
    {
        var pokemon = new Pokemon(
            Name: "test",
            Description: "irrelevant",
            Habitat: habitat,
            IsLegendary: isLegendary);

        var kind = PokemonTranslationPolicy.GetTranslationKind(pokemon);

        Assert.Equal(expected, kind);
    }
}
