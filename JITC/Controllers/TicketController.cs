using JITC.Models;
using JITC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JITC.Controllers
{
    public class TicketController : Controller
    {


        private readonly JITCSDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public TicketController(JITCSDbContext context, SignInManager<User> signInManager, UserManager<User> UserManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = UserManager;
        }

        [Authorize]
        public async Task<IActionResult> CreateTicket(int id)
        {
            if (id < 0)
            {
                return RedirectToAction("Index", "Route");
            }
            var route = await _context.Routes.Include(r => r.Pilote).Include(r => r.Helicopter).Include(r => r.StartAirport).Include(r => r.EndAirport).Where(r => r.RouteId == id).FirstOrDefaultAsync();

            if (route == null)
            {
                return RedirectToAction("Index", "Route");
            }

            if (route.Start.Date >= DateTime.Now.Date)
            {
                var airport = await _context.Airports.ToListAsync();


                var user = await _context.Users.Include(u => u.Messages).Where(u => u.Id == _userManager.GetUserId(HttpContext.User)).FirstOrDefaultAsync();
                var notification = user.Messages.Where(m => m.IsRead == false);
                return View(new InformationTicketViewModel()
                {

                    Id = route.RouteId,
                    Route = route,
                    Notification = notification.Count()
                });

            }

            return RedirectToAction("Index", "Route");

        }



        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTicket([Bind("Id,numberOfReservation")] InformationTicketViewModel information)
        {

            var route = await _context.Routes.Include(r => r.Pilote).Include(r => r.Helicopter).Include(r => r.StartAirport).Include(r => r.EndAirport).Where(r => r.RouteId == information.Id).FirstOrDefaultAsync();

            if (route == null)
            {
                return RedirectToAction("Index", "Route");
            }
            var airport = await _context.Airports.ToListAsync();
            var user = await _context.Users.Include(u => u.Messages).Where(u => u.Id == _userManager.GetUserId(HttpContext.User)).FirstOrDefaultAsync();
            var notification = user.Messages.Where(m => m.IsRead == false);
            var start = route.StartAirport;
            var end = route.EndAirport;

            if (ModelState.IsValid && route.Start.Date >= DateTime.Now)
            {
                if (information.numberOfReservation > 0 && information.numberOfReservation <= route.PlaceRemaining)
                {

                    route.CreateTicket(information.numberOfReservation);

                    Ticket ticket = new Ticket()
                    {
                        Place = information.numberOfReservation,
                        Route = route,
                        User = user,
                        Type = "Réserver",
                        Date = DateTime.Now

                    };

                    _context.Add(new Message()
                    {
                        Date = DateTime.Now,
                        Title = String.Format("Réservation pour le vol de {0}  à {1} ", start.Name, end.Name),
                        User = user,
                        Text = String.Format("Bonjour, {0} nous vous confirmons que le vol pour {1} et  à destination de {2} a  bien été réserver et que celui-ci décolle le {3} {4} et arrive à {5}.{6} Votre réservation comporte {7} places. {8} Bien à vous JITC",
                        Environment.NewLine, start.Name, end.Name, route.Start.ToString("dd/MM/yyyy"), route.PanifiedStart.ToString("HH:mm"), route.PanifiedEnd.ToString("HH:mm"), Environment.NewLine, information.numberOfReservation, Environment.NewLine),

                    });

                    _context.Add(ticket);
                    _context.SaveChanges();


                }

                return RedirectToAction("Index", "Route");

            }


            return View(new InformationTicketViewModel()
            {

                Id = route.RouteId,
                Route = route,
                Notification = notification.Count(),
                numberOfReservation = information.numberOfReservation
            });



        }



        [Authorize]
        public async Task<IActionResult> ListOfMyTicket()
        {

            var user = await _context.Users.Include(u => u.Messages).Where(u => u.Id == _userManager.GetUserId(HttpContext.User)).FirstOrDefaultAsync();
            var notification = user.Messages.Where(m => m.IsRead == false);
            var ticket = await _context.Ticket.Include(t => t.User).Include(t => t.Route).ThenInclude(r => r.StartAirport).Include(t => t.Route).ThenInclude(r => r.EndAirport).Include(t => t.Route).ThenInclude(r => r.Helicopter).Where(t => t.User.Id == user.Id && t.Type != "Annuler").ToListAsync();
            IList<TicketData> ticketData = new List<TicketData>();
            foreach (var item in ticket)
            {
                ticketData.Add(new TicketData()
                {
                    Route = item.Route,
                    Place = item.Place,
                    Id = item.IdTicket,
                    Start = item.Route.StartAirport,
                    End = item.Route.EndAirport,
                    Date = item.Date,
                    Type = item.Type,
                });
            }

            ticketData = ticketData.OrderByDescending(t => t.Route.Start.Date).ToList();


            return View(new MyTicketViewModel()
            {
                Notification = notification.Count(),
                Tickets = ticketData
            });
        }


        [Authorize]
        public async Task<IActionResult> CancelTicket(int id)
        {

            if (id < 0)
            {
                return RedirectToAction(nameof(ListOfMyTicket));
            }

            var ticket = await _context.Ticket.Where(t => t.IdTicket == id).FirstOrDefaultAsync();

            if (ticket == null)
            {
                return RedirectToAction(nameof(ListOfMyTicket));
            }

            var user = await _context.Users.Include(u => u.Messages).Where(u => u.Id == _userManager.GetUserId(HttpContext.User)).FirstOrDefaultAsync();

            var myTicket = await _context.Ticket.Include(t => t.User).Include(t => t.Route).ThenInclude(r => r.StartAirport).Include(t => t.Route).ThenInclude(r => r.EndAirport).Where(t => t.User.Id == user.Id).ToListAsync();


            if (myTicket.Where(r => r.IdTicket == ticket.IdTicket).FirstOrDefault() == null)
            {
                return RedirectToAction(nameof(ListOfMyTicket));
            }


            if (ticket.Type == "Annuler")
            {
                return RedirectToAction(nameof(ListOfMyTicket));
            }


            if (ticket.Route.Start.Date < DateTime.Now.Date)
            {
                return RedirectToAction(nameof(ListOfMyTicket));
            }




            ticket.Type = "Annuler";
            ticket.Route.CancelTicket(ticket.Place);
            _context.Add(new Message()
            {
                Date = DateTime.Now,
                Title = String.Format("Confimation de votre annulation du vol de {0}  à {1} ", ticket.Route.StartAirport.Name, ticket.Route.EndAirport.Name),
                Text = String.Format("Bonjour, {0} nous vous annonçons que le vol pour {1} et  à destination de {2} a bien été annuler.  Bien à vous JITC",
                  Environment.NewLine, ticket.Route.StartAirport.Name, ticket.Route.EndAirport.Name),
                User = user,
            });
            _context.SaveChanges();


            return RedirectToAction(nameof(ListOfMyTicket));

        }


        [Authorize]
        public async Task<IActionResult> HistoriqueTicket()
        {
            var user = await _context.Users.Include(u => u.Messages).Where(u => u.Id == _userManager.GetUserId(HttpContext.User)).FirstOrDefaultAsync();
            var notification = user.Messages.Where(m => m.IsRead == false);
            var ticket = await _context.Ticket.Include(t => t.User).Include(t => t.Route).ThenInclude(r => r.StartAirport).Include(t => t.Route).ThenInclude(r => r.EndAirport).Where(t => t.User.Id == user.Id ).ToListAsync();
            IList<TicketData> ticketData = new List<TicketData>();
            foreach (var item in ticket)
            {
                if (item.Type == "Annuler" || item.Type == "Terminé")
                {
                    ticketData.Add(new TicketData()
                    {
                        Route = item.Route,
                        Place = item.Place,
                        Id = item.IdTicket,
                        Start = item.Route.StartAirport,
                        End = item.Route.EndAirport,
                        Date = item.Date,
                        Type = item.Type,
                    });
                }

               
            }

            ticketData = ticketData.OrderByDescending(t => t.Route.Start.Date).ToList();


            return View(new MyTicketViewModel()
            {
                Notification = notification.Count(),
                Tickets = ticketData
            });
            return View();
        }




    }
}
