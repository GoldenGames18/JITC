using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace JITC.Models
{
    public class User : IdentityUser
    {
       

        [Required (ErrorMessage = "Le nom est requis")]
        public string Name { get; set; }

        [Required (ErrorMessage = "Le prénom est requis")]
        public string LastName { get; set; }   

        [Required(ErrorMessage = "L'Email est requis")]
        [EmailAddress(ErrorMessage = "Adresse Email est invalide")]
        public string Email { get; set; }

        public string Path { get; set; } = "~/img/user.png";

        public IList<Message>? Messages { get; set; }

      

    }
}
