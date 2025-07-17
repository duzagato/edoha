using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Entities
{
    public  class User : Entity
    {
        public string Name { get; set; }
        public string? Phone { get; set; }
        public string? Nickname { get; set; }
        public byte[]? PasswordUser { get; set; }
        public Guid IdUserType { get; set; }
    }
}
