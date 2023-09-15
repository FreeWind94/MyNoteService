using MyNoteService.DataLayer;
using MyNoteService.Model;

namespace MyNoteService.WebApi.Endpoints;

static class UsersEndpoints
{
    public static void AddUsersEndpoints(this WebApplication app)
    {
        var users = app.MapGroup("/users");
        users.MapGet("/", GetAllUsers).Produces<IEnumerable<User>>();
        users.MapGet("/{id:int}", GetUserById).Produces<User>();
        users.MapGet("/byLogin/{login}", GetUserByLogin).Produces<User>();
        users.MapPost("/", AddUser).Produces<User>();
        users.MapPut("/{id:int}", EditUser).Produces<User>();
        users.MapDelete("/{id:int}", DeleteUser);
    }

    public static async Task<IResult> GetAllUsers(IUserRepository userRepository)
    {
        var users = userRepository.GetEntities();
        return Results.Ok(users);
    }

    public static async Task<IResult> GetUserById(IUserRepository userRepository, int id)
    {
        var user = userRepository.GetEntityByID(id);

        if (user is null)
        {
            return Results.NotFound();
        }
        return Results.Ok(user);
    }

    public static async Task<IResult> GetUserByLogin(IUserRepository userRepository, string login)
    {
        var user = userRepository.GetEntityByLogin(login);

        if (user is null)
        {
            return Results.NotFound();
        }
        return Results.Ok(user);
    }

    public static async Task<IResult> AddUser(IUserRepository userRepository, User user)
    {
        userRepository.CreateEntity(user);
        return Results.Created($"users/{user.UserID}", user);
    }

    public static async Task<IResult> EditUser(IUserRepository userRepository, int id, User updatedUser)
    {
        var user = userRepository.GetEntityByID(id);

        if (user is null)
        {
            return Results.NotFound();
        }

        userRepository.EditEntity(updatedUser);
        return Results.Ok(updatedUser);
    }

    public static async Task<IResult> DeleteUser(IUserRepository userRepository, int id)
    {
        userRepository.DeleteEntity(id);
        return Results.Ok();
    }
}