using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Models.DTOs.User
{
    public class UserCredentials
    {
        public Guid? Id { get; set; }
        public string? Nickname { get; set; }
        public byte[]? Password { get; set; }
    }
}
