using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNoteService.DataLayer.SQL.Extensions;

internal static class TsqlExtensions
{
    /// <summary>
    /// Метод для добавления нового тега в заметку
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="noteId"></param>
    /// <param name="tagId"></param>
    public static void AddRowToNoteTags(this SqlConnection sql, int noteId, int tagId)
    {
        using var command = sql.CreateCommand();
        command.CommandText = "insert into [dbo].[NoteTags] ([NoteID], [TagID]) values (@noteID, @tagID); ";
        command.Parameters.AddWithValue("@noteID", noteId);
        command.Parameters.AddWithValue("@tagID", tagId);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Метод для удаления тега из заметки
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="noteId"></param>
    /// <param name="tagId"></param>
    public static void DeleteRowFromNoteTags(this SqlConnection sql, int noteId, int tagId)
    {
        using var command = sql.CreateCommand();
        command.CommandText = "delete from [dbo].[NoteTags] where [NoteID] = @noteID and [TagID] = @tagID; ";
        command.Parameters.AddWithValue("@noteID", noteId);
        command.Parameters.AddWithValue("@tagID", tagId);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Метод для добавления нового доступа в заметку
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="noteId"></param>
    /// <param name="userId"></param>
    public static void AddRowToAccess(this SqlConnection sql, int noteId, int userId)
    {
        using var command = sql.CreateCommand();
        command.CommandText = "insert into [dbo].[Access] ([NoteID], [UserID]) values (@noteID, @userID); ";
        command.Parameters.AddWithValue("@noteID", noteId);
        command.Parameters.AddWithValue("@userID", userId);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Метод для удаления доступа из заметки
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="noteId"></param>
    /// <param name="userId"></param>
    public static void DeleteRowFromAccess(this SqlConnection sql, int noteId, int userId)
    {
        using var command = sql.CreateCommand();
        command.CommandText = "delete from [dbo].[Access] where [NoteID] = @noteID and [UserID] = @userId; ";
        command.Parameters.AddWithValue("@noteID", noteId);
        command.Parameters.AddWithValue("@userId", userId);
        command.ExecuteNonQuery();
    }
}
