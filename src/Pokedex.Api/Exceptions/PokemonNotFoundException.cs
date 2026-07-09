namespace Pokedex.Api.Exceptions;

public class PokemonNotFoundException : Exception
{
    public PokemonNotFoundException()
        : base("The requested Pokemon was not found.") { }

    public PokemonNotFoundException(string pokemonName)
        : base($"The requested Pokemon was not found: {pokemonName}.") { }

    public PokemonNotFoundException(string pokemonName, Exception innerException)
        : base($"The requested Pokemon was not found: {pokemonName}.", innerException) { }
}
