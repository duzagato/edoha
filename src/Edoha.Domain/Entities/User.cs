using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Entities
{
    public  class User
    {
        public int IdUser { get; set; }
        public string NameUser { get; set; }
        public string PhoneUser { get; set; }
        public string NicknameUser { get; set; }
        public byte[] PasswordUser { get; set; }
        public int IdUserType { get; set; }
        public DateTime InsertionUser { get; set; } 
    }
}
