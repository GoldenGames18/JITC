using System.ComponentModel.DataAnnotations;

namespace JITC.CustomValidation
{
    public class ExtentionValidation : ValidationAttribute 
    {

        private readonly string[] _extensions;
        public ExtentionValidation(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var files = value as IFormFile;

            if (files != null)
            {
                var extension = Path.GetExtension(files.FileName);
                if (files != null)
                {
                    if (!_extensions.Contains(extension.ToLower()))
                    {
                        return new ValidationResult(GetErrorMessage(files.FileName));
                    }
                }
            }


                
            

            return ValidationResult.Success;
        }

        public string GetErrorMessage(string name)
        {
            return $"{name} Extension incorrect !";
        }



    }
}
