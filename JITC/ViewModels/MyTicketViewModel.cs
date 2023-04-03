using JITC.Models;

namespace JITC.ViewModels
{
    public class MyTicketViewModel
    {
        public IList<TicketData> Tickets { get; set; }

        public int Notification { get; set; }

    }
}
