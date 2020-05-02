using MyNoteService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyNoteService.DataLayer
{
    public interface INoteRepository : IRepository<Note>
    {
        
        
        // добавить что-то ещё???
        void AddNoteTag(int noteId, int tagId); // ???
        void RemoveNoteTag(int noteId, int tagId); // ???
        void AddUserWhithAccess(int noteId, int userId); // ???
        void RemoveUserWhithAccess(int noteId, int userId); // ???
    }
}
