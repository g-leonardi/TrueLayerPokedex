namespace Pokedex.Api.Exceptions;

/// <summary>Thrown when a Pokémon has no English flavor text to use as its description.</summary>
public class NoPokemonDescriptionException : Exception
{
    public NoPokemonDescriptionException()
        : base("No English description found for the Pokemon.") { }

    public NoPokemonDescriptionException(string pokemonName)
        : base($"No English description found for the Pokemon: {pokemonName}.") { }

    public NoPokemonDescriptionException(string pokemonName, Exception innerException)
        : base($"No English description found for the Pokemon: {pokemonName}.", innerException) { }
}
