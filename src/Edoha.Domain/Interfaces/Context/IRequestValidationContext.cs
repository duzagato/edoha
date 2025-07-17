using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Models.DTOs;

namespace Edoha.Domain.Interfaces.Context
{
    public interface IRequestValidationContext
    {
        Task AddError(string errorKey, string errorMessage);
        Dictionary<string, string> GetErrors();
        Task ValidateDTO(DTO dto);
    }
}
