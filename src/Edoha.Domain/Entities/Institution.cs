using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Entities
{
    public class Institution : Entity
    {
        [Required]
        public string Name { get; set; }
    }
}
