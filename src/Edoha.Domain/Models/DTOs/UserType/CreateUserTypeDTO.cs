using Edoha.Domain.Models.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Models.DTOs.UserType
{
    public class CreateUserTypeDTO : DTO
    {
        [Required]
        public string NameUserType { get; set; }
    }
}
