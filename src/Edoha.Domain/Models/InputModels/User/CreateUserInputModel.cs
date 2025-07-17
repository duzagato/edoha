using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Models.InputModels.User
{
    public class CreateUserInputModel
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Nickname { get; set; }
        public string? UnhashedPassword { get; set; }
        public Guid IdUserType { get; set; }
    }
}
