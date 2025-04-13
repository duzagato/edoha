using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Interfaces.Util
{
    public interface ICrypto
    {
        public Byte[] GetPBKDF2();
    }
}
