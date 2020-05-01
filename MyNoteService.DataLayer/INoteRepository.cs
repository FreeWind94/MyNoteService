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
        void AddNoteTag(Note note, Tag tag); // ???
        void RemoveNoteTag(Note note, Tag tag); // ???
        void AddUserWhithAccess(Note note, User user); // ???
        void RemoveUserWhithAccess(Note note, User user); // ???
    }
}
