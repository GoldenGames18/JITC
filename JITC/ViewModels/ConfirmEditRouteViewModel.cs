using JITC.CustomValidation;
using JITC.Models;
using System.ComponentModel.DataAnnotations;

namespace JITC.ViewModels
{
    public class ConfirmEditRouteViewModel
    {
        [Required]
        public int Id { get; set; }

        public IList<Helicopter>? Helicopters { get; set; }
        public IList<User>? Pilote { get; set; }




        [NumberValidator]
        [Required(ErrorMessage = "Aéroport de départ est requis")]
        public int StartAirport { get; set; }


        [Required(ErrorMessage = "Aéroport d'arrivée est requis")]
        [NumberValidator]
        public int EndAireport { get; set; }

        [Required(ErrorMessage = "Heure de départ requise")]
        [DataType(DataType.Time)]
        public DateTime StartFly { get; set; }


        [Required(ErrorMessage = "Heure d'arrivée requise")]
        [DataType(DataType.Time)]
        public DateTime EndFly { get; set; }


        [Required(ErrorMessage = "Date du vol requis")]
        [DataType(DataType.Date)]

        public DateTime DateFly { get; set; }


        [Required]
        public string Type { get; set; }

        [Required(ErrorMessage = "Un pilote est requis")]
        public string IdPilote { get; set; }

        [Required(ErrorMessage = "Un hélicoptère est requis")]
        [NumberValidator(ErrorMessage = "Merci de choisir un hélicoptère")]
        public int IdHelicopter { get; set; }

        [Required(ErrorMessage = "Nombre de places disponibles doit être indiqué ")]
        public int Size { get; set; }

    }
}
