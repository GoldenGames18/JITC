using System.ComponentModel.DataAnnotations;

namespace JITC.CustomValidation
{
    public class SizeValidation : ValidationAttribute
    {

        private readonly int _size;

        public SizeValidation(int value) 
        {
            _size = value;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var files = value as IFormFile;

            if (files != null)
            {

                if (files.Length > _size)
                {
                    return new ValidationResult(GetErrorMessage(files.FileName));
                }

            }

            return ValidationResult.Success;



        }

        public string GetErrorMessage(string name)
        {
            return $"{name} Taille trop volumineuse !";
        }



    }
}
