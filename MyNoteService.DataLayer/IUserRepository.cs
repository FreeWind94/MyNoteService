using MyNoteService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyNoteService.DataLayer
{
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// получение списка пользователей у которых есть доступ к заметке
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns></returns>
        IEnumerable<User> GetNoteAccessList(int noteId);
    }
}
