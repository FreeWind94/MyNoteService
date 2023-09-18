using MyNoteService.DataLayer.Helpers;
using MyNoteService.DataLayer.SQL.Extensions;
using MyNoteService.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace MyNoteService.DataLayer.SQL;

public class TsqlNoteRepository : INoteRepository
{
    private string _connectionString;

    private IUserRepository _userRepository;

    private ITagRepository _tagRepository;

    public TsqlNoteRepository(string connectionString, IUserRepository userRepository, ITagRepository tagRepository)
    {
        _connectionString = connectionString;
        _userRepository = userRepository;
        _tagRepository = tagRepository;
    }

    public Note CreateEntity(Note item)
    {
        // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
        using (var sql = new SqlConnection(_connectionString))
        {
            sql.Open();

            // создание новой заметки
            using (var command = sql.CreateCommand())
            {
                // предпологаем что гарантируется существование тегов и юзеров.

                // команда добавить заметку
                command.CommandText = "insert into [dbo].[Notes] ([UserId], [Topic], [Text]) output inserted.NoteID values (@userID, @topic, @text); ";
                command.Parameters.AddWithValue("@userID", item.Aurhor.UserID);
                command.Parameters.AddWithValue("@topic", item.Topic);
                command.Parameters.AddWithValue("@text", item.Text);

                // выполнение команды, получение id заметки
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new IOException("Insert query did not return Id");
                    }
                    item.NoteID = reader.GetInt32(reader.GetOrdinal("NoteID"));
                }
            }

            // установить тэги
            foreach (var tag in item.Tags)
            {
                sql.AddRowToNoteTags(item.NoteID, tag.TagID);
            }

            // установить связи доступа
            foreach (var user in item.UsersWhithAccess)
            {
                sql.AddRowToAccess(item.NoteID, user.UserID);
            }
        }

        // где-то тут, возможно, было бы правильно создать новый экземпряр с изменившимся ID
        // может стоит вообще достовать новый по Id, но решено сдалеть по простому и просто заменить ID в старом экземпляре и вернуть его же >.<
        return item;
    }

    public void DeleteEntity(int id)
    {
        Note note;
        try
        {
            note = GetEntityByID(id);
        }
        catch (Exception)
        {
            // TODO: залогировать проблему
            return;
        }
        if (note is null)
        {
            return;
        }

        // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
        using (var sql = new SqlConnection(_connectionString))
        {
            sql.Open();

            //SQL команда для удаления заметки
            using (var command = sql.CreateCommand())
            {
                command.CommandText = "delete from [dbo].[Notes] where [NoteID] = @noteID";
                command.Parameters.AddWithValue("@noteID", id);
                command.ExecuteNonQuery();
            }

            foreach (var tag in note.Tags)
            {
                sql.DeleteRowFromNoteTags(id, tag.TagID);
            }
            foreach (var user in note.UsersWhithAccess)
            {
                sql.DeleteRowFromAccess(id, user.UserID);
            }
        }
    }

    /// <summary>
    /// Метод EditEntity изменяет только заголовок и содержание заметки, 
    /// для изменения тэгов и доступа импользуются методы:
    /// AddNoteTag, 
    /// RemoveNoteTag, 
    /// AddUserWhithAccess, 
    /// RemoveUserWhithAccess
    /// </summary>
    /// <param name="updatedItem"></param>
    /// <returns></returns>
    public Note EditEntity(Note updatedItem)
    {
        var oldItem = GetEntityByID(updatedItem.NoteID);

        // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
        using (var sql = new SqlConnection(_connectionString))
        {
            sql.Open();

            //SQL команда для изменения заголовка и содержания заметки
            using (var command = sql.CreateCommand())
            {
                // "пока" сделаем по простому без проверок)
                command.CommandText = "update [dbo].[Notes] set [Topic] = @topic, [Text] = @text where [NoteID] = @noteID";
                command.Parameters.AddWithValue("@topic", updatedItem.Topic);
                command.Parameters.AddWithValue("@text", updatedItem.Text);
                command.Parameters.AddWithValue("@noteID", updatedItem.NoteID);
                command.ExecuteNonQuery();
            }

            (var deletedTags, var addedTags) = DataHelper.GetDeletedAndAdded(oldItem.Tags, updatedItem.Tags);
            foreach (var tag in deletedTags)
            {
                sql.DeleteRowFromNoteTags(updatedItem.NoteID, tag.TagID);
            }
            foreach (var tag in addedTags)
            {
                sql.AddRowToNoteTags(updatedItem.NoteID, tag.TagID);
            }

            (var deletedUsers, var addedUsers) = DataHelper.GetDeletedAndAdded(oldItem.UsersWhithAccess, updatedItem.UsersWhithAccess);
            foreach (var user in deletedUsers)
            {
                sql.DeleteRowFromAccess(updatedItem.NoteID, user.UserID);
            }
            foreach (var user in addedUsers)
            {
                sql.AddRowToAccess(updatedItem.NoteID, user.UserID);
            }
        }

        return updatedItem;
    }


    public IEnumerable<Note> GetEntities()
    {
        // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
        using (var sql = new SqlConnection(_connectionString))
        {
            sql.Open();

            //SQL команда вывода всех заметок
            using (var command = sql.CreateCommand())
            {
                command.CommandText = "select [NoteID], [UserId], [Topic], [Text] from [dbo].[Notes]";

                using (var reader = command.ExecuteReader())
                {
                    return ReadNotes(reader);
                }
            }
        }
    }

    public Note GetEntityByID(int id)
    {
        // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
        using (var sql = new SqlConnection(_connectionString))
        {
            sql.Open();

            //SQL команда вывода заметки
            using (var command = sql.CreateCommand())
            {
                command.CommandText = "select [NoteID], [UserId], [Topic], [Text] from [dbo].[Notes] where [NoteID] = @noteID";
                command.Parameters.AddWithValue("@noteID", id);

                using (var reader = command.ExecuteReader())
                {
                    return ReadNote(reader);
                }
            }
        }
    }

    private Note ReadNote(SqlDataReader reader)
    {
        if (!reader.Read())
        {
            return null;
        }
        Note note = new Note
        {
            NoteID = reader.GetInt32(reader.GetOrdinal("NoteID")),
            Aurhor = _userRepository.GetEntityByID(reader.GetInt32(reader.GetOrdinal("UserId"))),
            Topic = reader.GetString(reader.GetOrdinal("Topic")),
            Text = reader.GetString(reader.GetOrdinal("Text"))
        };
        note.Tags = _tagRepository.GetNoteTags(note.NoteID);
        note.UsersWhithAccess = _userRepository.GetUsersWhithAccess(note.NoteID);

        return note;
    }

    private IEnumerable<Note> ReadNotes(SqlDataReader reader)
    {
        List<Note> result = new List<Note>();

        while (reader.Read())
        {
            Note note = new Note
            {
                NoteID = reader.GetInt32(reader.GetOrdinal("NoteID")),
                Aurhor = _userRepository.GetEntityByID(reader.GetInt32(reader.GetOrdinal("UserId"))),
                Topic = reader.GetString(reader.GetOrdinal("Topic")),
                Text = reader.GetString(reader.GetOrdinal("Text"))
            };
            note.Tags = _tagRepository.GetNoteTags(note.NoteID);
            note.UsersWhithAccess = _userRepository.GetUsersWhithAccess(note.NoteID);

            result.Add(note);
        }

        return result;
    }

    public void AddNoteTag(int noteId, int tagId)
    {
        // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
        using var sql = new SqlConnection(_connectionString);
        sql.Open();
        sql.AddRowToNoteTags(noteId, tagId);
    }

    public void RemoveNoteTag(int noteId, int tagId)
    {
        // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
        using var sql = new SqlConnection(_connectionString);
        sql.Open();
        sql.DeleteRowFromNoteTags(noteId, tagId);
    }

    public void AddUserWhithAccess(int noteId, int userId)
    {
        // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
        using var sql = new SqlConnection(_connectionString);
        sql.Open();
        sql.AddRowToAccess(noteId, userId);
    }

    public void RemoveUserWhithAccess(int noteId, int userId)
    {
        // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
        using var sql = new SqlConnection(_connectionString);
        sql.Open();
        sql.DeleteRowFromAccess(noteId, userId);
    }
}
