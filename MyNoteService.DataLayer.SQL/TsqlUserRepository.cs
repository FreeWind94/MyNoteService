using MyNoteService.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace MyNoteService.DataLayer.SQL
{
    public class TsqlUserRepository : IUserRepository
    {
        private string _connectionString;

        public TsqlUserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User CreateEntity(User item)
        {
            // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
            using (var sql = new SqlConnection(_connectionString))
            {
                sql.Open();

                // SQL команда на сервер дря создания нового юзера
                using (var command = sql.CreateCommand())
                {
                    command.CommandText = "insert into [dbo].[Users] output Inserted.UserID values (@loginName, @userPassword)";
                    command.Parameters.AddWithValue("@loginName", item.LoginName);
                    command.Parameters.AddWithValue("@userPassword", item.UserPassword);

                    using (var reader = command.ExecuteReader())
                    {
                        // возможно это стоит вынести в отдельный метод так-как этот код имеет высокий шанс изменится 
                        if (!reader.Read())
                        {
                            throw new IOException("Insert query did not return Id");
                        }
                        item.UserID = reader.GetInt32(reader.GetOrdinal("UserID"));
                    }
                }
            }

            // где-то тут, возможно, было бы правильно создать новый экземпряр Tag с изменившимся ID
            // может стоит вообще достовать новый по Id, но решено сдалеть по простому и просто заменить ID в старом экземпляре и вернуть его же >.<
            return item;
        }

        public void DeleteEntity(int id)
        {
            // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
            using (var sql = new SqlConnection(_connectionString))
            {
                sql.Open();

                //SQL команда для удаления юзера
                using(var command = sql.CreateCommand())
                {
                    command.CommandText = "delete from [dbo].[Users] where [UserID] = @userID";
                    command.Parameters.AddWithValue("@userID", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteEntity(string loginName)
        {
            // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
            using (var sql = new SqlConnection(_connectionString))
            {
                sql.Open();

                //SQL команда для удаления юзера
                using (var command = sql.CreateCommand())
                {
                    command.CommandText = "delete from [dbo].[Users] where [LoginName] = @loginName";
                    command.Parameters.AddWithValue("@loginName", loginName);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Крайне небезопасная херня, будем считать что этот метод 
        /// изменяет исключительно loginName у юзера с заданным UserID
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public User EditEntity(User item)
        {
            // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
            using (var sql = new SqlConnection(_connectionString))
            {
                sql.Open();

                //SQL команда для изменения логина юзера
                using (var command = sql.CreateCommand())
                {
                    // "пока" сделаем по простому без проверок)
                    command.CommandText = "update [dbo].[Users] set [LoginName] = @loginName where [UserID] = @userID";
                    command.Parameters.AddWithValue("@loginName", item.LoginName);
                    command.Parameters.AddWithValue("@userID", item.UserID);
                    command.ExecuteNonQuery();
                }
            }

            return item;  // бесит
        }

        public IEnumerable<User> GetEntities()
        {
            // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
            using (var sql = new SqlConnection(_connectionString))
            {
                sql.Open();

                //SQL команда вывода всех юзеров
                using (var command = sql.CreateCommand())
                {
                    command.CommandText = "select [UserID], [LoginName], [UserPassword] from [dbo].[Users]";

                    using (var reader = command.ExecuteReader())
                    {
                        return ReadUsers(reader);
                    }
                }
            }

        }

        public User GetEntityByID(int id)
        {
            // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
            using (var sql = new SqlConnection(_connectionString))
            {
                sql.Open();

                //SQL команда вывода юзера
                using (var command = sql.CreateCommand())
                {
                    command.CommandText = "select [UserID], [LoginName], [UserPassword] from [dbo].[Users] where [UserID] = @userID";
                    command.Parameters.AddWithValue("@userID", id);

                    using (var reader = command.ExecuteReader())
                    {
                        return ReadUser(reader);
                    }
                }
            }
        }

        public User GetEntityByLogin(string login)
        {
            // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
            using (var sql = new SqlConnection(_connectionString))
            {
                sql.Open();

                //SQL команда вывода юзера
                using (var command = sql.CreateCommand())
                {
                    command.CommandText = "select [UserID], [LoginName], [UserPassword] from [dbo].[Users] where [LoginName] = @loginName";
                    command.Parameters.AddWithValue("@loginName", login);

                    using (var reader = command.ExecuteReader())
                    {
                        return ReadUser(reader);
                    }
                }
            }
        }

        private static User ReadUser(SqlDataReader reader)
        {
            if (!reader.Read())
            {
                throw new IOException("Insert query did not return User");
            }
            User user = new User
            {
                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                LoginName = reader.GetString(reader.GetOrdinal("LoginName")),
                UserPassword = reader.GetString(reader.GetOrdinal("UserPassword"))
            };

            return user;
        }

        public User IsUserAuthorized(string login, string password)
        {
            // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
            using (var sql = new SqlConnection(_connectionString))
            {
                sql.Open();

                //SQL команда вывода юзера
                using (var command = sql.CreateCommand())
                {
                    command.CommandText = "select [UserID], [LoginName], [UserPassword] from [dbo].[Users] where [LoginName] = @loginName AND [UserPassword] = @userPassword";
                    command.Parameters.AddWithValue("@loginName", login);
                    command.Parameters.AddWithValue("@userPassword", password);

                    using (var reader = command.ExecuteReader())
                    {
                        return ReadUser(reader);
                    }
                }
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
    }
}
