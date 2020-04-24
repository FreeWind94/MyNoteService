﻿using MyNoteService.Model;
using System;
using System.IO;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace MyNoteService.DataLayer.SQL
{
    public class TsqlTagRepository : ITagRepository
    {
        private string _connectionString;

        public TsqlTagRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Tag CreateEntity(Tag item)
        {
            
            // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
            using (var sql = new SqlConnection(_connectionString))
            {
                sql.Open();

                // SQL команда на сервер дря создания нового тега
                using(var command = sql.CreateCommand())
                {
                    command.CommandText = "insert into [dbo].[Tags] output Inserted.TagID values (@name)";
                    command.Parameters.AddWithValue("@name", item.TagName);

                    using (var reader = command.ExecuteReader())
                    {
                        // возможно это стоит вынести в отдельный метод так-как этот код имеет высокий шанс изменится 
                        if(!reader.Read())
                        {
                            throw new IOException("Insert query did not return Id");
                        }
                        item.TagID = reader.GetInt32(reader.GetOrdinal("TagID"));
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

                //SQL команда для удаления тега
                using (var command = sql.CreateCommand())
                {
                    command.CommandText = "delete from [dbo].[Tags] where [TagID] = @tagID";
                    command.Parameters.AddWithValue("@tagID", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Изменяется только имя тега (по его ID)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Tag EditEntity(Tag item)
        {
            // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
            using (var sql = new SqlConnection(_connectionString))
            {
                sql.Open();

                //SQL команда для изменения имени тега
                using (var command = sql.CreateCommand())
                {
                    // "пока" сделаем по простому без проверок)
                    command.CommandText = "update [dbo].[Tags] set [TagName] = @tagName where [TagID] = @tagID";
                    command.Parameters.AddWithValue("@tagName", item.TagName);
                    command.Parameters.AddWithValue("@tagID", item.TagID);
                    command.ExecuteNonQuery();
                }
            }

            return item;
            throw new NotImplementedException();
        }

        public IEnumerable<Tag> GetEntities()
        {
            // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
            using (var sql = new SqlConnection(_connectionString))
            {
                sql.Open();

                //SQL команда вывода всех тегов
                using (var command = sql.CreateCommand())
                {
                    command.CommandText = "select [TagID], [TagName] from [dbo].[Tags]";

                    using (var reader = command.ExecuteReader())
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
                }
            }
        }

        public Tag GetEntityByID(int id)
        {
            // соединение с базой (внутри блока using, чтобы потом она сама закрылась)
            using (var sql = new SqlConnection(_connectionString))
            {
                sql.Open();

                //SQL команда вывода всех тегов
                using (var command = sql.CreateCommand())
                {
                    command.CommandText = "select [TagID], [TagName] from [dbo].[Tags] where [TagID] = @tagID";
                    command.Parameters.AddWithValue("@tagID", id);

                    using (var reader = command.ExecuteReader())
                    {
                        return ReadTag(reader);
                    }
                }
            }
        }

        private static Tag ReadTag(SqlDataReader reader)
        {
            if (!reader.Read())
            {
                throw new IOException("Insert query did not return Tag");
            }
            Tag tag = new Tag
            {
                TagID = reader.GetInt32(reader.GetOrdinal("TagID")),
                TagName = reader.GetString(reader.GetOrdinal("TagName"))
            };

            return tag;
        }
    }
}
