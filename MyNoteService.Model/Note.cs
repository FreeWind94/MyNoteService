using System;
using System.Collections.Generic;
using System.Text;

namespace MyNoteService.Model
{
    public class Note
    {
        public int NoteID { get; set; }
        public User Aurhor { get; set; }
        public string Topic { get; set; }
        public string Text { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public IEnumerable<User> UsersWhithAccess { get; set; }
    }
}
