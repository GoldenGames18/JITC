using System.ComponentModel.DataAnnotations;

namespace JITC.Models
{
    public class Airport
    {
        [Key]
        public int AirportId { get; set; }
        [Required(ErrorMessage = "Le nom de l'aéroport est requis")]
        public string Name { get; set; }
        [Required(ErrorMessage = "La latitude de l'aéroport est requis")]
        public string Latitude { get; set; }
        [Required(ErrorMessage = "La longitude de l'aéroport est requis")]
        public string Longitude { get; set; } 
        
   





    }
}
