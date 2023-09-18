using System;
using System.Collections.Generic;
using System.Text;

namespace MyNoteService.Model
{
    public class User
    {
        public int UserID { get; set; }
        public string LoginName { get; set; }
        public string UserPassword { get; set; } // TODO: наверное это нужно хранить не так но эту проблему я оставляю будущему мне

        public override bool Equals(object obj)
        {
            if (obj is User user)
            {
                return UserID == user.UserID;
            }
            return base.Equals(obj);
        }
    }
}
