using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Models.DTOs.UserPermission
{
    public class CreateUserPermissionDTO : DTO
    {
        [Required]
        public Guid IdUser { get; set; }

        [Required]
        public Guid IdPage {  get; set; }

        [Required]
        public Guid IdPermission { get; set; }

        [Required]
        public Guid IdAction { get; set; }
    }
}
