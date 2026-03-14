using Edoha.Domain.Models.DTOs;
using Edoha.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;
using Edoha.Domain.Interfaces.Infraestructure.Context;

namespace Edoha.Infraestructure.Context
{
    public class RequestValidationContext : IRequestValidationContext
    {
        private Dictionary<string, string> Errors { get; set; }
        public bool IsValid => Errors.Count == 0;

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

            bool isValid = !IsValid
            ? false
            : Validator.TryValidateObject(this, context, results, validateAllProperties: true);


            if (!isValid && results.Count > 0)
            {
                ProccessValidationResult(results);
                throw new RequestValidationException(GetErrors());
            }
        }

        private void ProccessValidationResult(List<ValidationResult>? results)
        {
            if (results != null && results.Count > 0)
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
