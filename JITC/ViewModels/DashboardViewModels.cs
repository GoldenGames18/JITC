using JITC.Models;

namespace JITC.ViewModels
{
    public class DashboardViewModels
    {

        public int NumberUser { get; set; } 

        public int NumberHelyco { get; set; }

        public int NumberRoute { get; set; }

        public int NumberPilote { get; set; } 

        public IList<User> Pilote { get; set; }


    }
}
