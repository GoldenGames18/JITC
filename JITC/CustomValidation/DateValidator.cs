using System.ComponentModel.DataAnnotations;

namespace JITC.CustomValidation
{
    public class DateValidator : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var date = Convert.ToDateTime(value.ToString());

            if (date == DateTime.MinValue)
            {
                return new ValidationResult(GetErrorMessage("Données requises"));
            }
            else if (DateTime.Compare(date, DateTime.Now) < 0)
            {
                return new ValidationResult(GetErrorMessage("Date antérieure"));
            }


            return ValidationResult.Success;
        }

        public string GetErrorMessage(string name)
        {
            return $"{name}  !";
        }


    }
}
