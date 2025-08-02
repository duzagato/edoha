using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Entities
{
    public class UserPermission
    {
        public Guid IdUser { get; set; }
        public Guid IdPage { get; set; }
        public Guid IdPermission { get; set; }
        public Guid IdAction { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}
