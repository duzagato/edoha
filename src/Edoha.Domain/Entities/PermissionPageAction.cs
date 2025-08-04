using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Entities
{
    public class PermissionPageAction
    {
        public Guid IdPermission { get; set; }
        public Guid IdPage { get; set; }
        public Guid IdAction { get; set; }
    }
}
