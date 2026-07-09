using Microsoft.Extensions.Options;
using Pokedex.Api.Infra.ApiClients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<PokemonApiOptions>(
    builder.Configuration.GetSection(PokemonApiOptions.SectionName));

builder.Services.AddHttpClient<IPokemonApiClient, PokemonApiClient>((sp, client) =>
{
    var opt = sp.GetRequiredService<IOptions<PokemonApiOptions>>().Value;
    client.BaseAddress = new Uri(opt.BaseUrl);
});

var app = builder.Build();

app.UseHttpsRedirection();
app.Run();

