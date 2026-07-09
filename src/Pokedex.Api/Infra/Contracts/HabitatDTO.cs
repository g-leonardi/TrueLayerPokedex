using System.Text.Json.Serialization;
namespace Pokedex.Api.Infra.Contracts;

public record HabitatDTO
(
    [property: JsonPropertyName("name")]
    string Name

);
  
