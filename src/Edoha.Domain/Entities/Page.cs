using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Entities
{
    public class Page : Entity
    {
        public string Name {  get; set; }
        public string? Description { get; set; }
    }
}
