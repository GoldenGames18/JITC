using System.ComponentModel.DataAnnotations;

namespace JITC.ViewModels
{
    public class ReportViewModel
    {

        public int? Notification { get; set; }

        public JITC.Models.Route? Route { get; set; }

        [Required]
        public int IdRoute { get; set; }

        [Required(ErrorMessage = "Heure de départ requise")]
        [DataType(DataType.Time)]
        public DateTime Start { get; set; }


        [Required(ErrorMessage = "Heure d'arrivée requise")]
        [DataType(DataType.Time)]
        public DateTime End { get; set; }

        public string? Message { get; set; }
    }
}
