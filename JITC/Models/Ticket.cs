using System.ComponentModel.DataAnnotations;

namespace JITC.Models
{
    public class Ticket
    {
        [Key]
        public int IdTicket { get; set; } 
        public Route Route { get; set; }
        public User User { get; set; }
        public int Place { get; set; }

        public DateTime Date { get; set; }

        public string Type { get; set; } //annuler / réserver / terminé

    }
}
