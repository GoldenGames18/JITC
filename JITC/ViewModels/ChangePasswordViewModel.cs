using System.ComponentModel.DataAnnotations;

namespace JITC.ViewModels
{
    public class ChangePasswordViewModel
    {


        [Required(ErrorMessage = "Mot de passe requis")]
        [DataType(DataType.Password)]
        public string ActualPassword { get; set; }

        [Required(ErrorMessage = "Mot de passe requis")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmation du mot de passe requis")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Les deux mots de passe ne sont pas identiques")]
        public string ConfirmPassword { get; set; }



        public string? Role { get; set; }
        public int Notification { get; set; } = 0;
        public string? Path { get; set; }

    }
}
