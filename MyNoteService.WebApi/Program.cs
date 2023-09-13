using MyNoteService.DataLayer;
using MyNoteService.DataLayer.SQL;
using MyNoteService.WebApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// TODO: избавиться от хардкода
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

app.AddNotesEndpoints();
app.AddUsersEndpoints();

app.Run();
