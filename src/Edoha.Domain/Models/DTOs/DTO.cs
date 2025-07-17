using Edoha.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Models.DTOs
{
    public abstract class DTO
    {
        public IEnumerable<PropertyInfo> GetProperties(string idColumnName)
        {
            return GetType()
                   .GetProperties()
                   .Where(p => p.Name != idColumnName);
        }
    }
}
