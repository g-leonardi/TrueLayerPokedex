namespace Pokedex.Api.Infra.ApiClients.PokeApi;

public class PokemonApiOptions
{
    public const string SectionName = "PokemonApi";
    public string BaseUrl { get; set; } = "";   // e.g. "https://pokeapi.co/api/v2/"
}
