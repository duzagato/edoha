using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Entities
{
    public class UserInstitution
    {
        public Guid IdUser { get; set; }
        public Guid IdInstitution { get; set; }

        public DateTime CreateAt { get; set; }

        public Guid? CreateBy { get; set; }
    }
}
