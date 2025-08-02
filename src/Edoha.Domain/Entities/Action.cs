using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Entities
{
    public class Action : Entity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool WithoutOwner {  get; set; }
        public bool OtherOwner { get; set; }
    }
}
