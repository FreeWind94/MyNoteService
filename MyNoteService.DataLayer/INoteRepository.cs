using MyNoteService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyNoteService.DataLayer
{
    public interface INoteRepository : IRepository<Note>
    {
        // вывод тегов заметки
        IEnumerable<Tag> GetNoteTags(int noteId);
        //  вывод юзеров с доступом к заметке
        IEnumerable<User> GetUsersWhithAccess(int noteId);
        // добавить что-то ещё???
    }
}
