using System;
using System.Collections.Generic;

namespace Edoha.Domain.Exceptions
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
