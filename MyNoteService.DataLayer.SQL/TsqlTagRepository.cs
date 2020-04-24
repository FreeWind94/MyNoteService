using MyNoteService.Model;
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
            throw new NotImplementedException();
        }

        public Tag EditEntity(Tag item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tag> GetEntities()
        {
            throw new NotImplementedException();
        }

        public Tag GetEntityByID(int id)
        {
            throw new NotImplementedException();
        }
    }
}
