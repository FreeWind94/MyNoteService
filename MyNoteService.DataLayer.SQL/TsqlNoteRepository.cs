using MyNoteService.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace MyNoteService.DataLayer.SQL
{
    public class TsqlNoteRepository : INoteRepository
    {
        private string _connectionString;

        private IUserRepository _userRepository; //?????

        private ITagRepository _tagRepository; //????????

        public TsqlNoteRepository(string connectionString, IUserRepository userRepository, ITagRepository tagRepository) // TODO: разобраться с _userRepository, _tagRepository
        {
            _connectionString = connectionString;
            _userRepository = userRepository;
            _tagRepository = tagRepository;
        }

        public void AddNoteTag(Note note, Tag tag)
        {
            throw new NotImplementedException();
        }

        public void AddUserWhithAccess(Note note, User user)
        {
            throw new NotImplementedException();
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
                    // TODO: ...
                    // предпологаем что гарантируется существование тегов и юзеров.

                    // команда добавить заметку
                    command.CommandText = "insert into [dbo].[Notes] ([UserId], [Topic], [Text]) output inserted.NoteID values (@userID, @topic, @text); ";
                    command.Parameters.AddWithValue("@userID", item.Aurhor.UserID);
                    command.Parameters.AddWithValue("@topic", item.Topic);
                    command.Parameters.AddWithValue("@text", item.Text);
                    
                    // выполнение команды, получение id заметки
                    using(var reader = command.ExecuteReader())
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
                    using (var command = sql.CreateCommand())
                    {
                        command.CommandText = "insert into [dbo].[NoteTags] ([NoteID], [TagID]) values (@noteID, @tagID); ";
                        command.Parameters.AddWithValue("@noteID", item.NoteID);
                        command.Parameters.AddWithValue("@tagID", tag.TagID);
                        command.ExecuteNonQuery();
                    }
                }

                // установить связи доступа
                foreach (var user in item.UsersWhithAccess)
                {
                    using (var command = sql.CreateCommand())
                    {
                        command.CommandText = "insert into [dbo].[Access] ([NoteID], [UserID]) values (@noteID, @userID); ";
                        command.Parameters.AddWithValue("@noteID", item.NoteID);
                        command.Parameters.AddWithValue("@userID", user.UserID);
                        command.ExecuteNonQuery();
                    }
                }
            }

            // где-то тут, возможно, было бы правильно создать новый экземпряр с изменившимся ID
            // может стоит вообще достовать новый по Id, но решено сдалеть по простому и просто заменить ID в старом экземпляре и вернуть его же >.<
            return item;
        }

        public void DeleteEntity(int id)
        {

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
            }
        }

        /// <summary>
        /// Метод EditEntity изменяет только заголовок и содержание заметки, для изменения тэгов и доступа импользуются методы ... 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Note EditEntity(Note item)
        {
            // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
            using (var sql = new SqlConnection(_connectionString))
            {
                sql.Open();

                //SQL команда для изменения заголовка и содержания заметки
                using (var command = sql.CreateCommand())
                {
                    // "пока" сделаем по простому без проверок)
                    command.CommandText = "update [dbo].[Notes] set [Topic] = @topic, [Text] = @text where [NoteID] = @noteID";
                    command.Parameters.AddWithValue("@topic", item.Topic);
                    command.Parameters.AddWithValue("@text", item.Text);
                    command.Parameters.AddWithValue("@noteID", item.NoteID);
                    command.ExecuteNonQuery();
                }
            }

            return item;  
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
                        // TODO: зарефакторить этот ужас
                        while (reader.Read())
                        {
                            Note note = new Note
                            {
                                NoteID = reader.GetInt32(reader.GetOrdinal("NoteID")),
                                Aurhor = _userRepository.GetEntityByID(reader.GetInt32(reader.GetOrdinal("UserId"))),
                                Topic = reader.GetString(reader.GetOrdinal("Topic")),
                                Text = reader.GetString(reader.GetOrdinal("Text"))
                            };
                            note.Tags = this.GetNoteTags(note.NoteID);
                            note.UsersWhithAccess = this.GetUsersWhithAccess(note.NoteID);

                            yield return note;
                        }
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

            throw new NotImplementedException();
        }

        private Note ReadNote(SqlDataReader reader)
        {
            if (!reader.Read())
            {
                throw new IOException("Insert query did not return User");
            }
            Note note = new Note
            {
                NoteID = reader.GetInt32(reader.GetOrdinal("NoteID")),
                Aurhor = _userRepository.GetEntityByID(reader.GetInt32(reader.GetOrdinal("UserId"))),
                Topic = reader.GetString(reader.GetOrdinal("Topic")),
                Text = reader.GetString(reader.GetOrdinal("Text"))
            };
            note.Tags = this.GetNoteTags(note.NoteID);
            note.UsersWhithAccess = this.GetUsersWhithAccess(note.NoteID);

            return note;
        }

        public IEnumerable<Tag> GetNoteTags(int noteId)
        {
            // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
            using (var sql = new SqlConnection(_connectionString))
            {
                sql.Open();

                //SQL команда вывода всех тегов заметки с указаным id
                using (var command = sql.CreateCommand())
                {
                    command.CommandText = "select t.TagID, t.TagName from [dbo].[Tags] as t join [dbo].[NoteTags] as nt on nt.TagID = t.TagID where NoteID = @noteID;";
                    command.Parameters.AddWithValue("@noteID", noteId);

                    using (var reader = command.ExecuteReader())
                    {
                        return ReadTags(reader);
                    }
                }
            }
        }

        private static IEnumerable<Tag> ReadTags(SqlDataReader reader)
        {
            while (reader.Read())
            {
                Tag tag = new Tag
                {
                    TagID = reader.GetInt32(reader.GetOrdinal("TagID")),
                    TagName = reader.GetString(reader.GetOrdinal("TagName"))
                };

                yield return tag;
            }
        }

        public IEnumerable<User> GetUsersWhithAccess(int noteId)
        {
            // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
            using (var sql = new SqlConnection(_connectionString))
            {
                sql.Open();

                //SQL команда вывода всех юзеров у которых есть доступ к заметке с указаным id
                using (var command = sql.CreateCommand())
                {
                    command.CommandText = "select u.UserID, u.LoginName, u.UserPassword from [dbo].[Users] as u join [dbo].[Access] as a on a.UserID = u.UserID where NoteID = @noteID;";
                    command.Parameters.AddWithValue("@noteID", noteId);

                    using (var reader = command.ExecuteReader())
                    {
                        return ReadUsers(reader);
                    }
                }
            }
        }

        private static IEnumerable<User> ReadUsers(SqlDataReader reader)
        {
            while (reader.Read())
            {
                User user = new User
                {
                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                    LoginName = reader.GetString(reader.GetOrdinal("LoginName")),
                    UserPassword = reader.GetString(reader.GetOrdinal("UserPassword"))
                };

                yield return user;
            }
        }

        public void RemoveNoteTag(Note note, Tag tag)
        {
            throw new NotImplementedException();
        }

        public void RemoveUserWhithAccess(Note note, User user)
        {
            throw new NotImplementedException();
        }
    }
}
