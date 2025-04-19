using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Models.Requests
{
    public abstract class Request
    {
        public void Validate()
        {
            var context = new ValidationContext(this, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(this, context, results, validateAllProperties: true);

            if (!isValid)
            {
                string errors = string.Join("; ", results.Select(r => r.ErrorMessage));
                throw new ValidationException($"Erro de validação: {errors}");
            }
        }

        public IEnumerable<PropertyInfo> GetProperties(string idColumnName)
        {
            return GetType()
                   .GetProperties()
                   .Where(p => p.Name != idColumnName);
        }
    }
}
