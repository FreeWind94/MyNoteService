using System;
using System.Collections.Generic;
using System.Text;

namespace MyNoteService.Model
{
    public class Tag
    {
        public int TagID { get; set; }
        public string TagName { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Tag tag)
            {
                return TagID == tag.TagID;
            }
            return base.Equals(obj);
        }
    }
}
