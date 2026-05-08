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

        [Required]
        public string SlugName { get; set; }
        
        [Required]
        public string ShortName { get; set; }
        
        public string? Description { get; set; }

        public string? LogoDirectory { get; set; }
    }
}
