using System;
using System.ComponentModel.DataAnnotations;

namespace Edoha.Domain.Annotations.String
{
    public class MinLength : ValidationAttribute
    {
        private readonly int _minLength;

        public MinLength(int minLength)
        {
            _minLength = minLength;
            ErrorMessage = $"O campo deve ter no mínimo {_minLength} caracteres.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || value.ToString()?.Length < _minLength)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
