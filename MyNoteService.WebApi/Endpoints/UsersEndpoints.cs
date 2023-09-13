using MyNoteService.DataLayer;
using MyNoteService.DataLayer.SQL;
using MyNoteService.Model;

namespace MyNoteService.WebApi.Endpoints;

static class UsersEndpoints
{
    public static void AddUsersEndpoints(this WebApplication app)
    {
        app.MapGet("/users", GetAllUsers).Produces<IEnumerable<User>>();
        app.MapGet("/users/{id:int}", GetUserById).Produces<User>();
        app.MapGet("/users/byLogin/{login}", GetUserByLogin).Produces<User>();
        app.MapPost("/users", AddUser).Produces<User>();
        app.MapPut("/users/{id:int}", EditUser).Produces<User>();
        app.MapDelete("/users/{id:int}", DeleteUser);
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