using Edoha.Domain.Models.DTOs;
using Edoha.Shared.Exceptions;
using Edoha.Domain.Interfaces.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Infraestructure.Context
{
    public class RequestValidationContext : IRequestValidationContext
    {
        private Dictionary<string, string> Errors { get; set; }
        public bool IsValid => Errors.Count <= 1;

        public RequestValidationContext()
        {
            Errors = new Dictionary<string, string>();
        }

        public async Task AddError(string errorKey, string errorMessage)
        {
            if (!Errors.ContainsKey(errorKey))
            {
                Errors.Add(errorKey, errorMessage);
            }
        }

        public Dictionary<string, string> GetErrors()
        {
            return Errors;
        }

        public async Task ValidateDTO(DTO dto)
        {
            var context = new ValidationContext(this, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(this, context, results, validateAllProperties: true);

            if (!isValid)
            {
                ProccessValidationResult(results);
                throw new RequestValidationException(GetErrors());
            }
        }

        private void ProccessValidationResult(List<ValidationResult>? results)
        {
            if (results != null)
            {
                foreach (var result in results)
                {
                    SetErrors(result);
                }
            }
        }

        private void SetErrors(ValidationResult result)
        {
            foreach (var memberName in result.MemberNames)
            {
                string errorKey = memberName;
                string errorValue = result.ErrorMessage ?? "Ocorreu um erro, tente novamente";
                AddError(errorKey, errorValue);
            }
        }
    }
}
