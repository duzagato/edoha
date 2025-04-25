using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Models.InputModels.User
{
    public class CreateUserInputModel
    {
        public string? NameUser { get; set; }
        public string? PhoneUser { get; set; }
        public string? NicknameUser { get; set; }
        public string? UnhashedPassword { get; set; }
        public int IdUserType { get; set; }
    }
}
