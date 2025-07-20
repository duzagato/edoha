using Edoha.Domain.Models.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Models.DTOs.User
{
    public class CreateUserDTO : DTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }
        public string? Nickname { get; set; }
        public byte[]? Password { get; set; }
        public Guid IdUserType { get; set; }
    }
}
