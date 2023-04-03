using System.ComponentModel.DataAnnotations;

namespace JITC.Models
{
    public class Message
    {

        [Key]
        public int IdMessage { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime Date { get; set; }
        public User User { get; set; }
    }
}
