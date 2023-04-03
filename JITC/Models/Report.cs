using System.ComponentModel.DataAnnotations;

namespace JITC.Models
{
    public class Report
    {
        [Key]
        public int IdRaport { get; set; }

        public Route Route { get; set; }
        
        [Required(ErrorMessage = "Heure de départ requise")]
        [DataType(DataType.Time)]
        public DateTime Start { get; set; }


        [Required(ErrorMessage = "Heure d'arrivée requise")]
        [DataType(DataType.Time)]
        public DateTime End { get; set; }

        public string? Message { get; set; }

    }
}
