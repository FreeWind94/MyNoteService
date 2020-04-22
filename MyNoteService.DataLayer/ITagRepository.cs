using MyNoteService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyNoteService.DataLayer
{
    public interface ITagRepository : IRepository<Tag>
    {
        /// <summary>
        /// получение всех тегов заметки
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns></returns>
        IEnumerable<Tag> GetNoteTags(int noteId);
    }
}
