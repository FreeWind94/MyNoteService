using MyNoteService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyNoteService.DataLayer
{
    public interface IUserRepository : IRepository<User>
    {
        User GetEntityByLogin(string login);

        User IsUserAuthorized(string login, string password); //пока так

        void DeleteEntity(string loginName);

        //  вывод юзеров с доступом к заметке
        IEnumerable<User> GetUsersWhithAccess(int noteId);
    }
}
