using JITC.Models;
using JITC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JITC.Controllers
{
    public class ReportController : Controller
    {

        private readonly JITCSDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public ReportController(JITCSDbContext context, SignInManager<User> signInManager, UserManager<User> UserManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = UserManager;
        }


        [Authorize(Roles = "Pilote")]
        public async Task<IActionResult> CreateRaport(int id)
        {
            if (id < 0)
            {
                return RedirectToAction("RoutePilote", "Route");
            }

            var route = await _context.Routes.Include(i => i.Pilote).Include(i => i.Helicopter).Include(r => r.StartAirport).Include(r => r.EndAirport).Where(r => r.RouteId == id).FirstOrDefaultAsync();

            if (route == null)
            {
                return RedirectToAction("RoutePilote", "Route");
            }

            var user = await _context.Users.Include(u => u.Messages).Where(u => u.Id == _userManager.GetUserId(HttpContext.User)).FirstOrDefaultAsync();
            var notification = user.Messages.Where(m => m.IsRead == false);

            if (route.Pilote.Id != user.Id)
            {
                return RedirectToAction("RoutePilote", "Route");
            }


            if (route.Finish)
            {
                return RedirectToAction("RoutePilote", "Route");
            }




            return View(new ReportViewModel()
            {
                Notification = notification.Count(),
                IdRoute = route.RouteId,
                Route = route
            });
        }

        [Authorize(Roles = "Pilote")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRaport([Bind("IdRoute,Start,End,Message")] ReportViewModel report)
        {

            if (report.IdRoute < 0)
            {
                return RedirectToAction("RoutePilote", "Route");
            }

            var route = await _context.Routes.Include(i => i.Pilote).Include(i => i.Helicopter).Include(r => r.StartAirport).Include(r => r.EndAirport).Where(r => r.RouteId == report.IdRoute).FirstOrDefaultAsync();

            if (route == null)
            {
                return RedirectToAction("RoutePilote", "Route");
            }

            var user = await _context.Users.Include(u => u.Messages).Where(u => u.Id == _userManager.GetUserId(HttpContext.User)).FirstOrDefaultAsync();
            var notification = user.Messages.Where(m => m.IsRead == false);

            if (route.Pilote.Id != user.Id)
            {
                return RedirectToAction("RoutePilote", "Route");
            }


            if (route.Finish)
            {
                return RedirectToAction("RoutePilote", "Route");
            }

            report.Route = route;
            report.IdRoute = route.RouteId;
            report.Notification = notification.Count();

            if (ModelState.IsValid)
            {

                if (report.End.TimeOfDay >= route.PanifiedEnd.TimeOfDay && report.End.TimeOfDay <= route.PanifiedEnd.AddMinutes(5).TimeOfDay && report.Start.TimeOfDay >= route.PanifiedStart.TimeOfDay && report.Start.TimeOfDay <= route.PanifiedStart.AddMinutes(5).TimeOfDay)
                {

                    _context.Reports.Add(new Report()
                    {
                        End = report.End,
                        Start = report.Start,
                        Message = report.Message,
                        Route = route,
                    });
                    var listTicket = _context.Ticket.Include(t => t.Route).Where(t => t.Route.RouteId == route.RouteId).ToList();
                    foreach (var item in listTicket)
                    {
                        item.Type = "Terminée";
                    }

                    route.Finish = true;

                    _context.SaveChanges();

                    return RedirectToAction("RoutePilote", "Route");
                }
                else
                {
                    if (string.IsNullOrEmpty(report.Message))
                    {
                        ViewBag.Error = "Un message est requis car il semble avoir du retard";
                        return View(report);
                    }
                    else
                    {
                        _context.Reports.Add(new Report()
                        {
                            End = report.End,
                            Start = report.Start,
                            Message = report.Message,
                            Route = route,
                        });

                        var listTicket = _context.Ticket.Include(t => t.Route).Where(t => t.Route.RouteId == route.RouteId).ToList();
                        foreach (var item in listTicket)
                        {
                            item.Type = "Terminée";
                        }

                        route.Finish = true;
                        _context.SaveChanges();
                    }



                }

            }
            return View(report);

        }



        ///Admin part 
        ///
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListOfReport()
        {
            var reports = await _context.Reports.Include(r => r.Route).ThenInclude(r => r.EndAirport).Include(r => r.Route).ThenInclude(r => r.StartAirport).ToListAsync();
            return View( reports.OrderByDescending(r => r.Route.Start).ToList() );

        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Information(int id)
        {
            if (id < 0)
            {
                return RedirectToAction(nameof(ListOfReport));
            }

            var report = await _context.Reports.Include(r => r.Route).ThenInclude(r => r.EndAirport).Include(r => r.Route).ThenInclude(r => r.StartAirport).Include(r => r.Route).ThenInclude(r => r.Pilote).Include(r => r.Route).ThenInclude(r => r.Helicopter)
                .FirstOrDefaultAsync(u => u.IdRaport == id);


           


            if (report == null)
            {
                return RedirectToAction(nameof(ListOfReport));
            }

            return View(report);
        }


    }
}
