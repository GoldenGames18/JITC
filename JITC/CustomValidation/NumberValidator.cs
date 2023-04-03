using System.ComponentModel.DataAnnotations;

namespace JITC.CustomValidation
{
    public class NumberValidator : ValidationAttribute
    {
        public string ErrorMessage { get; set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var number = Convert.ToInt64(value);

            if (number == 0)
                return new ValidationResult(GetErrorMessage(ErrorMessage));
            return ValidationResult.Success;
        }

        public string GetErrorMessage(string name)
        {
            return $"{name} !";
        }


    }
}
