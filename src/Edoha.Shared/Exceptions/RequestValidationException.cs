using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Shared.Exceptions
{
    public class RequestValidationException : Exception
    {
        public Dictionary<string, string> Errors { get; }

        public RequestValidationException(Dictionary<string, string> errors)
        {
            Errors = errors;
        }
    }
}
