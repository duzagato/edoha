using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Entities
{
    internal class User
    {
        public int IdUser { get; set; }
        public string NameUser { get; set; } // name_user
        public string PhoneUser { get; set; } // phone_user
        public string NicknameUser { get; set; } // nickname_user
        public byte[] PasswordUser { get; set; } // password_user (varbinary)
        public int IdUserType { get; set; } // id_user_type
        public DateTime InsertionUser { get; set; } // insertion_user
    }
}
