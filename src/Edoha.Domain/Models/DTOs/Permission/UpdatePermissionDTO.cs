using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Models.DTOs.Permission
{
    public class UpdatePermissionDTO : DTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
