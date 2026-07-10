using Microsoft.Extensions.Options;
using Pokedex.Api.Exceptions;
using Pokedex.Api.Infra.ApiClients.PokeApi;
using Pokedex.Api.Infra.ApiClients.Translation;
using Pokedex.Api.Infra.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<PokemonApiOptions>(
    builder.Configuration.GetSection(PokemonApiOptions.SectionName));

builder.Services.Configure<TranslationApiOptions>(
    builder.Configuration.GetSection(TranslationApiOptions.SectionName));

builder.Services.AddHttpClient<IPokemonApiClient, PokemonApiClient>((sp, client) =>
{
    var opt = sp.GetRequiredService<IOptions<PokemonApiOptions>>().Value;
    client.BaseAddress = new Uri(opt.BaseUrl);
});

// Translation client talks to two absolute URLs (from TranslationApiOptions),
// so no single BaseAddress here — the client posts to the URL it resolves per kind.
builder.Services.AddHttpClient<ITranslationApiClient, TranslationApiClient>();

builder.Services.AddScoped<IPokemonService, PokemonService>();
var app = builder.Build();

app.UseHttpsRedirection();
//First endopoint
app.MapGet("/pokemon/{name}", async (string name, IPokemonService service, CancellationToken ct) =>
{
    try
    {
      var pokemon = await service.GetPokemonAsync(name, ct);
      return Results.Ok(pokemon);  
    }
    catch (PokemonNotFoundException)
    {
        return Results.NotFound();
    }
    catch (HttpRequestException)
    {
        return Results.StatusCode(502);
    }
});

//Second endpoint
app.MapGet("/pokemon/translated/{name}", async (string name, IPokemonService service, CancellationToken ct) =>
{
    try
    {
      var pokemon = await service.GetPokemonTranslatedAsync(name, ct);
      return Results.Ok(pokemon);  
    }
    catch (PokemonNotFoundException)
    {
        return Results.NotFound();
    }
    catch (HttpRequestException)
    {
        return Results.StatusCode(502);
    }
});

app.Run();

// Exposes the implicit top-level Program class to the test project (WebApplicationFactory<Program>).
public partial class Program { }

