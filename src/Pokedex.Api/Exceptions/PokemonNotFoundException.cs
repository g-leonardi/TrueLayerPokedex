namespace Pokedex.Api.Exceptions;

/// <summary>Thrown when the requested Pokémon does not exist (PokéAPI returns 404). Maps to a 404 response.</summary>
public class PokemonNotFoundException : Exception
{
    public PokemonNotFoundException()
        : base("The requested Pokemon was not found.") { }

    public PokemonNotFoundException(string pokemonName)
        : base($"The requested Pokemon was not found: {pokemonName}.") { }

    public PokemonNotFoundException(string pokemonName, Exception innerException)
        : base($"The requested Pokemon was not found: {pokemonName}.", innerException) { }
}
