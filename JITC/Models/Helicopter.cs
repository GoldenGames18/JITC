using System.ComponentModel.DataAnnotations;

namespace JITC.Models
{
    public class Helicopter
    {
        [Key]
        public int HelicopterId { get; set; }

        [Required(ErrorMessage = "Nom de l'hélicoptère requis")]
        public string HelicopterName { get; set; }

        [Required(ErrorMessage = "Capacité cabine est requise")]

        public int Size { get; set; }
        [Required(ErrorMessage = "Vitesse est requise")]

        public int Speed { get; set; }
        [Required(ErrorMessage = "Référence du moteur est requise")]
        public string Engine { get; set; }
        public int Fly { get; set; } = 0;



        public void ResetFly() 
        {
            Fly = 0;
        }
    }
}
