using JITC.CustomValidation;
using JITC.Models;
using System.ComponentModel.DataAnnotations;
using System.Device.Location;

namespace JITC.ViewModels
{
    public class UniqueRouteViewModel
    {


        public IList<Airport>? Aiports { get; set; }

        [NumberValidator(ErrorMessage = "Merci de choisir un aéroport")]
        [Required(ErrorMessage = "Aéroport de départ est requis")]
        public int StartAirport { get; set; }
        [Required(ErrorMessage = "Aéroport d'arrivée est requis")]
        [NumberValidator(ErrorMessage = "Merci de choisir un aéroport")]
        public int EndAireport { get; set; }

        [Required(ErrorMessage = "Heure de départ requise")]
        [DataType(DataType.Time)]


        public DateTime StartFly { get; set; }


        [Required(ErrorMessage = "Heure d'arrivée requise")]
        [DataType(DataType.Time)]

        public DateTime EndFly { get; set; }
        [Required(ErrorMessage = "Date du vol requis")]
        [DataType(DataType.Date)]
        [DateValidator]
        public DateTime DateFly { get; set; }

        public string Type { get; set; } = "Ticket Unique";




    }
}
