using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Models.DTOs.Page
{
    public class UpdatePage
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
