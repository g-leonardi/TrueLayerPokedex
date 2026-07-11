namespace Pokedex.Api.Infra.ApiClients.PokeApi;

/// <summary>Configuration for the PokéAPI client, bound from the <c>PokemonApi</c> section.</summary>
public class PokemonApiOptions
{
    public const string SectionName = "PokemonApi";

    /// <summary>Base URL of the PokéAPI, e.g. <c>https://pokeapi.co/api/v2/</c>.</summary>
    public string BaseUrl { get; set; } = "";
}
