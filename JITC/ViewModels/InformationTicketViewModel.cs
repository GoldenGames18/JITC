using JITC.CustomValidation;
using JITC.Models;
using System.ComponentModel.DataAnnotations;

namespace JITC.ViewModels
{
    public class InformationTicketViewModel
    {
        [Required]
        public int Id { get; set; }

        public Models.Route? Route { get; set; }

        public int? Notification { get; set; }

        [Required(ErrorMessage = "Merci d'indiquer le nombre de réservations")]
        public int numberOfReservation { get; set; } = 1;
    }
}
