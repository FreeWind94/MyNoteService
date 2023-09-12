using Microsoft.AspNetCore.Mvc;
using MyNoteService.DataLayer;
using MyNoteService.DataLayer.SQL;
using MyNoteService.Model;

var builder = WebApplication.CreateBuilder(args);

// TODO: избавиться от харткода
var connectionString = "Data Source=DESKTOP-H9KPRLI;Initial Catalog=notesdb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
builder.Services.AddSingleton<IUserRepository>(provider => new TsqlUserRepository(connectionString));
builder.Services.AddSingleton<ITagRepository>(provider => new TsqlTagRepository(connectionString));
builder.Services.AddSingleton<INoteRepository>(provider => new TsqlNoteRepository(connectionString, 
    provider.GetService<IUserRepository>(), 
    provider.GetService<ITagRepository>()));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/notes", ([FromServices]INoteRepository repo) => {
    return repo.GetEntities();
});

app.MapGet("/notes/{id}", ([FromServices] INoteRepository repo, int id) => {
    var note = repo.GetEntityByID(id);
    return note is not null
        ? Results.Ok(note)
        : Results.NotFound();
});

app.MapPost("/notes", ([FromServices] INoteRepository repo, Note note) =>
{
    repo.CreateEntity(note);
    return Results.Created($"notes/{note.NoteID}", note);
});

app.MapPut("/notes/{id}", ([FromServices] INoteRepository repo, int id, Note updatedNote) =>
{
    var note = repo.GetEntityByID(id);
    if (note is null)
    {
        return Results.NotFound();
    }

    repo.EditEntity(updatedNote);
    return Results.Ok(updatedNote);
});

app.MapDelete("/notes/{id}", ([FromServices] INoteRepository repo, int id) => {
    repo.DeleteEntity(id);
    return Results.Ok(); 
});

app.Run();
