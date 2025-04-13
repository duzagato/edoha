using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Annotations.Numerical
{
    public class Min : ValidationAttribute
    {
        private readonly int _min;

        public Min(int min)
        {
            _min = min;
            ErrorMessage = $"O campo permite apenas valores acima de 0.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            try
            {
                decimal num = Convert.ToDecimal(value);

                if (num >= _min)
                {
                    Console.WriteLine("Aqui");
                    return ValidationResult.Success;
                }
                else
                {
                    Console.WriteLine("Bola");
                    return new ValidationResult(ErrorMessage);
                }
            }catch(Exception ex)
            {
                return new ValidationResult(ex.Message);
            }
            
        }
    }
}
