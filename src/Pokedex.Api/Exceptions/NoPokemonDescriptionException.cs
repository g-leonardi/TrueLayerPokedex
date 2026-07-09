namespace Pokedex.Api.Exceptions;

public class NoPokemonDescriptionException : Exception
{
    public NoPokemonDescriptionException()
        : base("No English description found for the Pokemon.") { }

    public NoPokemonDescriptionException(string pokemonName)
        : base($"No English description found for the Pokemon: {pokemonName}.") { }

    public NoPokemonDescriptionException(string pokemonName, Exception innerException)
        : base($"No English description found for the Pokemon: {pokemonName}.", innerException) { }
}