using JITC.Models;
using Route = JITC.Models.Route;

namespace JITC.ViewModels
{
    public class ListOfCurrentRoute
    {

       public IList<Route> CurrentRoute { get; set; }

        public int Notification { get; set; } = 0;


    }
}
