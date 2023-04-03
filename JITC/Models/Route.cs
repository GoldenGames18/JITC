using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JITC.Models
{
    public class Route
    {

        [Key]
        public int RouteId { get; set; }
        [Required(ErrorMessage = "Hélicoptère manquant")]
        public Helicopter Helicopter  { get; set; }


        [Required(ErrorMessage = "Pilote manquant")]
        public User Pilote { get; set; }

       
        [Required(ErrorMessage = "Aéroport de départ manquant")]
        public Airport StartAirport { get; set; }

       
        [Required(ErrorMessage = "Aéroport de fin manquant")]
        public Airport EndAirport { get; set; }

        [Required(ErrorMessage = "Nombre de places manquante")]
        public int Place { get; set; }

        [DataType(DataType.Time)]
        [Required(ErrorMessage = "Date de départ planifiée  manquante")]
        public DateTime PanifiedStart { get; set; }

        [DataType(DataType.Time)]
        [Required(ErrorMessage = "Date d'arrivée planifiée  manquante")]
        public DateTime PanifiedEnd { get; set; }

        public double Kilometre { get; set; }

        public DateTime Start { get; set; } 

        [Required]
        public string Type { get; set; } 
        public bool Finish { get; set; }

        public int PlaceRemaining { get; set; }


        public void CreateTicket(int number)
        { 
            PlaceRemaining -= number;
        }


        public void CancelTicket(int number)
        { 
            PlaceRemaining += number;
        }


    }
}
