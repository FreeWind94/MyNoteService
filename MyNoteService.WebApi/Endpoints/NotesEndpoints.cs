using Microsoft.AspNetCore.Mvc;
using MyNoteService.DataLayer;
using MyNoteService.Model;

namespace MyNoteService.WebApi.Endpoints;

static class NotesEndpoints
{
    public static void AddNotesEndpoints(this WebApplication app)
    {
        var notes = app.MapGroup("/notes");
        notes.MapGet("/", GetAllNotes).Produces<IEnumerable<Note>>();
        notes.MapGet("/{id:int}", GetNoteById).Produces<Note>();
        notes.MapPost("/", AddNote).Produces<Note>();
        notes.MapPut("/{id:int}", EditNote).Produces<Note>();
        notes.MapDelete("/{id:int}", DeleteNote);
    }

    public static async Task<IResult> GetAllNotes(INoteRepository noteRepository)
    {
        var notes = noteRepository.GetEntities();
        return Results.Ok(notes);
    }

    public static async Task<IResult> GetNoteById(INoteRepository noteRepository, int id)
    {
        var note = noteRepository.GetEntityByID(id);
        return note is not null
            ? Results.Ok(note)
            : Results.NotFound();
    }

    public static async Task<IResult> AddNote(INoteRepository noteRepository, Note note)
    {
        noteRepository.CreateEntity(note);
        return Results.Created($"notes/{note.NoteID}", note);
    }

    public static async Task<IResult> EditNote(INoteRepository noteRepository, int id, Note updatedNote)
    {
        var note = noteRepository.GetEntityByID(id);
        if (note is null)
        {
            return Results.NotFound();
        }

        noteRepository.EditEntity(updatedNote);
        return Results.Ok(updatedNote);
    }

    private static async Task<IResult> DeleteNote(INoteRepository noteRepository, int id)
    {
        noteRepository.DeleteEntity(id);
        return Results.Ok();
    }
}