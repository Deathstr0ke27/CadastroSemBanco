using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
 
// Lista em memória para armazenar os filmes inciais
var FilmesLista = new List<Filmes.API.Model.Filmes>
{
    new Filmes.API.Model.Filmes { Id = 1, Nome = "Wonka", Genero = "Musical", Plataforma = "Max", Ano = 2023, IsAssistido = true },
    new Filmes.API.Model.Filmes { Id = 2, Nome = "Jogos Vorazes", Genero = "Ficção Científica", Plataforma = "Netflix", Ano = 2012, IsAssistido = true },
    new Filmes.API.Model.Filmes { Id = 3, Nome = "Parasita", Genero = "Drama", Plataforma = "Prime Video", Ano = 2019, IsAssistido = false }
};

builder.Services.AddSingleton(FilmesLista);
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Filmes.API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
 
app.UseHttpsRedirection();



// Rota GET para obter todos os produtos
app.MapGet("/filmes", () => 
{
    var filmesService = app.Services.GetRequiredService<List<Filmes.API.Model.Filmes>>();

    if (filmesService.Count == 0) { return Results.NotFound(); }

    return Results.Ok(filmesService);

});

// Rota GET para obter filmes já assistidos
app.MapGet("/assistidos", () => 
{
    var filmesService = app.Services.GetRequiredService<List<Filmes.API.Model.Filmes>>();
    var assistidos = filmesService.FindAll(g => g.IsAssistido == true);

    if (assistidos.Count == 0) { return Results.NotFound(); }

    return Results.Ok(assistidos);

});

// Rota GET para obter um produto pelo Id
app.MapGet("/filmes/{id}", (int id, HttpRequest request) =>
{
    var filmesService = app.Services.GetRequiredService<List<Filmes.API.Model.Filmes>>();
    var filme = filmesService.FirstOrDefault(p => p.Id == id);
    if (filme == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(filme);
});

// Rota POST para criar um novo produto
app.MapPost("/filmes", (Filmes.API.Model.Filmes filme) =>
{
    var filmesService = app.Services.GetRequiredService<List<Filmes.API.Model.Filmes>>();
    filme.Id = filmesService.Max(f => f.Id) + 1;
    filmesService.Add(filme);
    return Results.Created($"/filmes/{filme.Id}", filme);
});

// Rota PUT para atualizar um produto existente
app.MapPut("/filmes/{id}", (int id, Filmes.API.Model.Filmes filme) =>
{
    var filmesService = app.Services.GetRequiredService<List<Filmes.API.Model.Filmes>>();
    var existingFilme = filmesService.FirstOrDefault(f => f.Id == id);

    if (existingFilme == null)
    {
        return Results.NotFound();
    }

    existingFilme.Nome = filme.Nome;
    existingFilme.Genero = filme.Genero;
    existingFilme.Plataforma = filme.Plataforma;
    existingFilme.Ano = filme.Ano;
    existingFilme.IsAssistido = filme.IsAssistido;

    return Results.NoContent();
});

// Rota PUT para marcar como assistido
app.MapPut("/assistido/{id}/{action}", (int id, string action) =>
{
    var filmesService = app.Services.GetRequiredService<List<Filmes.API.Model.Filmes>>();
    var filme = filmesService.FirstOrDefault(f => f.Id == id);

    if (filme == null)
    {
        return Results.NotFound();
    }

    if (action == "assistido") filme.IsAssistido = true;
    else if (action == "desconhecido") filme.IsAssistido = false;
    else return Results.BadRequest("opção inválida, digite 'assistido' ou 'desconhecido'.");

    return Results.Ok();
});

// Rota DELETE para excluir um produto
app.MapDelete("/filmes/{id}", (int id) =>
{
    var filmesService = app.Services.GetRequiredService<List<Filmes.API.Model.Filmes>>();
    var existingFilme = filmesService.FirstOrDefault(f => f.Id == id);

    if (existingFilme == null)
    {
        return Results.NotFound();
    }

    filmesService.Remove(existingFilme);
    
    return Results.NoContent();
});

app.Run();