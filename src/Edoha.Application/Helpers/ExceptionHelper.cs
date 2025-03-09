using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Application.Helpers
{
    public class ExceptionHelper
    {
        public static void ValidateDTO(object dto)
        {
            var context = new ValidationContext(dto, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(dto, context, results, validateAllProperties: true);

            if (!isValid)
            {
                string errors = string.Join("; ", results.Select(r => r.ErrorMessage));
                throw new ValidationException($"Erro de validação: {errors}");
            }
        }
    }
}
