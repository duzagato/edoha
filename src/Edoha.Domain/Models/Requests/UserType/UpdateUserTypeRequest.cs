using Edoha.Domain.Models.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Models.Requests.UserType
{
    public class UpdateUserTypeRequest : Request
    {
        [Required]
        public string? NameUserType { get; set; }
    }
}
