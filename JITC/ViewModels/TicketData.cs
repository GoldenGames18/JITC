using JITC.Models;
using System;
using Route = JITC.Models.Route;

namespace JITC.ViewModels
{
    public class TicketData
    {
        public int Id    { get; set; }
        public Route Route { get; set; }
 
        public int Place { get; set; }

        public string Type { get; set; }
        public DateTime Date { get; set; }
        public Airport Start { get; set; }
        public Airport End { get; set; }

    }
}
