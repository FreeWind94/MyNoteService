using MyNoteService.DataLayer;
using MyNoteService.Model;

namespace MyNoteService.WebApi.Endpoints;

public static class TagsEndpoints
{
    public static void AddTagsEndpoints(this WebApplication app)
    {
        var tags = app.MapGroup("/tags");
        tags.MapGet("/", GetAllTags).Produces<IEnumerable<Tag>>();
        tags.MapGet("/{id:int}", GetTagById).Produces<Tag>();
        tags.MapGet("/fromNote/{id:int}", GetNoteTags).Produces<Tag>();
        tags.MapPost("/", AddTag);
        tags.MapPut("/{id:int}", EditTag);
        tags.MapDelete("/{id:int}", DeleteTag);
    }

    public static async Task<IResult>GetAllTags(ITagRepository tagRepository)
    {
        var tags = tagRepository.GetEntities();
        return Results.Ok(tags);
    }

    public static async Task<IResult> GetTagById(ITagRepository tagRepository, int id)
    {
        var tag = tagRepository.GetEntityByID(id);
        return tag is not null 
            ? Results.Ok(tag)
            : Results.NotFound();
    }

    public static async Task<IResult> GetNoteTags(ITagRepository tagRepository, int id)
    {
        var tags = tagRepository.GetNoteTags(id);
        return Results.Ok(tags);
    }

    public static async Task<IResult> AddTag(ITagRepository tagRepository, Tag tag)
    {
        tagRepository.CreateEntity(tag);
        return Results.Created($"/tags/{tag.TagID}", tag);
    }

    public static async Task<IResult> EditTag(ITagRepository tagRepository, int id, Tag updatedTag)
    {
        var tag = tagRepository.GetNoteTags(id);
        if (tag is null)
        {
            return Results.NotFound();
        }
        tagRepository.EditEntity(updatedTag);
        return Results.Ok(updatedTag);
    }

    public static async Task<IResult> DeleteTag(ITagRepository tagRepository, int id)
    {
        tagRepository.DeleteEntity(id);
        return Results.Ok();
    }
}
