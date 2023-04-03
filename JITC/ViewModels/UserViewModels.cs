using JITC.CustomValidation;
using JITC.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace JITC.ViewModels
{
    public class UserViewModels
    {
        [Required(ErrorMessage = "Le nom est requis")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Le prénom est requis")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "L'Email est requis")]
        [EmailAddress(ErrorMessage = "Adresse Email est invalide")]
        public string Email { get; set; }

        public string? Path { get; set; }

        public string? Role { get; set; }

        [SizeValidation(10000000)]
        [ExtentionValidation(new string[] { ".jpg", ".png" })]
        public IFormFile? File { get; set; }


        public int Notification { get; set; }
    }
}
