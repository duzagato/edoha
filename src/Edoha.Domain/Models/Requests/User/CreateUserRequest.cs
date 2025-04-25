using Edoha.Domain.Models.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Models.Requests.User
{
    public class CreateUserRequest : Request
    {
        public string NameUser { get; set; }
        public string PhoneUser { get; set; }
        public string NicknameUser { get; set; }
        public byte[] PasswordUser { get; set; }
        public int IdUserType { get; set; }
    }
}
