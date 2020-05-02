using MyNoteService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyNoteService.DataLayer
{
    public interface ITagRepository : IRepository<Tag>
    {
        // вывод тегов заметки
        IEnumerable<Tag> GetNoteTags(int noteId);
    }
}
