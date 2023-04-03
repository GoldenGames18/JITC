using JITC.Models;
using JITC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Device.Location;
using System.Linq;
using Route = JITC.Models.Route;

namespace JITC.Controllers
{
    public class RouteController : Controller
    {

        private readonly JITCSDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public RouteController(JITCSDbContext context, SignInManager<User> signInManager, UserManager<User> UserManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = UserManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListOfRoute()
        {
            IList<RouteViewModels> routes = new List<RouteViewModels>();

            var listOfroute = await _context.Routes.Include(r => r.Pilote).Include(r => r.Helicopter).Include(r => r.StartAirport).Include(r => r.EndAirport).Where(r => r.Finish == false).ToListAsync();

            var allHelico = await _context.Airports.ToListAsync();

            foreach (var route in listOfroute)
            {
                routes.Add(new RouteViewModels() { RouteId = route.RouteId,
                    Kilometre = route.Kilometre, Place = route.Place,
                    End = route.StartAirport.Name,
                    Start = route.EndAirport.Name,
                    PlaceRemaining = route.PlaceRemaining,
                    Date = route.Start

                });

            }


            return View(routes);
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRoute()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UniqueTicket()
        {
            UniqueRouteViewModel uniqueRouteViewModel = new UniqueRouteViewModel();
            uniqueRouteViewModel.Aiports = await _context.Airports.ToListAsync();
            return View(uniqueRouteViewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UniqueTicket([Bind("StartAirport,EndAireport,StartFly,EndFly,DateFly")] UniqueRouteViewModel uniqueRouteViewModel)
        {

            if (ModelState.IsValid)
            {

                return RedirectToAction("CreateTicketUnique", uniqueRouteViewModel);
            }

            uniqueRouteViewModel.Aiports = await _context.Airports.ToListAsync();
            return View(uniqueRouteViewModel);
        }


        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateTicketUnique(UniqueRouteViewModel? uniqueRouteViewModel)
        {
            CreateUniqueRouteViewModel createUniqueRouteViewModel = new CreateUniqueRouteViewModel();
            if (uniqueRouteViewModel != null)
            {
                createUniqueRouteViewModel.DateFly = uniqueRouteViewModel.DateFly;
                createUniqueRouteViewModel.EndFly = uniqueRouteViewModel.EndFly;
                createUniqueRouteViewModel.StartFly = uniqueRouteViewModel.StartFly;
                createUniqueRouteViewModel.EndAireport = uniqueRouteViewModel.EndAireport;
                createUniqueRouteViewModel.StartAirport = uniqueRouteViewModel.StartAirport;
                createUniqueRouteViewModel.Type = uniqueRouteViewModel.Type;
            }
            var routes = await _context.Routes.Include(r => r.Pilote).Include(r => r.Helicopter).ToArrayAsync();
            var sameDay = routes.Where(r => r.Start.Date == createUniqueRouteViewModel.DateFly.Date);
            IList<Route> sameHour = new List<Route>();
            foreach (var route in sameDay)
            {
                if (createUniqueRouteViewModel.StartFly.Hour <= route.PanifiedStart.Hour && createUniqueRouteViewModel.EndFly.Hour >= route.PanifiedStart.Hour || createUniqueRouteViewModel.EndFly.Hour <= route.PanifiedStart.Hour && createUniqueRouteViewModel.EndFly.Hour >= route.PanifiedStart.Hour)
                {
                    sameHour.Add(route);
                }
            }
            var helico = await _context.Helicopters.ToListAsync();
            var pilote = await _context.Roles.Where(r => r.Name == "Pilote").FirstAsync();
            var listOfPiloteId = await _context.UserRoles.Where(r => r.RoleId == pilote.Id).ToListAsync();
            IList<User> pilot = new List<User>();
            foreach (var item in listOfPiloteId)
            {
                pilot.Add(await _context.Users.Where(u => u.Id == item.UserId).FirstAsync());
            }
            foreach (var item in sameHour)
            {
                pilot.Remove(item.Pilote);
                helico.Remove(item.Helicopter);
            }
            createUniqueRouteViewModel.Helicopters = helico;
            createUniqueRouteViewModel.Pilote = pilot;
            return View(createUniqueRouteViewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTicketUnique([Bind("StartAirport,EndAireport,StartFly,EndFly,DateFly,Type,IdPilote,IdHelicopter,Size")] CreateUniqueRouteViewModel createUniqueRouteViewModel)
        {
            var helico = await _context.Helicopters.ToListAsync();
            var pilote = await _context.Roles.Where(r => r.Name == "Pilote").FirstAsync();
            var listOfPiloteId = await _context.UserRoles.Where(r => r.RoleId == pilote.Id).ToListAsync();
            IList<User> pilot = new List<User>();
            foreach (var item in listOfPiloteId)
            {
                pilot.Add(await _context.Users.Where(u => u.Id == item.UserId).FirstAsync());
            }
            var routes = await _context.Routes.Include(r => r.Pilote).Include(r => r.Helicopter).ToArrayAsync();
            var sameDay = routes.Where(r => r.Start.Date == createUniqueRouteViewModel.StartFly.Date);
            IList<Route> sameHour = new List<Route>();
            foreach (var route in sameDay)
            {
                if (createUniqueRouteViewModel.StartFly.Hour <= route.PanifiedStart.Hour && createUniqueRouteViewModel.EndFly.Hour >= route.PanifiedStart.Hour || createUniqueRouteViewModel.EndFly.Hour <= route.PanifiedStart.Hour && createUniqueRouteViewModel.EndFly.Hour >= route.PanifiedStart.Hour)
                {
                    sameHour.Add(route);
                }
            }
            foreach (var item in sameHour)
            {
                pilot.Remove(item.Pilote);
                helico.Remove(item.Helicopter);
            }
            createUniqueRouteViewModel.Helicopters = helico;
            createUniqueRouteViewModel.Pilote = pilot;


            if (ModelState.IsValid)
            {

                var myHelico = helico.Where(h => h.HelicopterId == createUniqueRouteViewModel.IdHelicopter).FirstOrDefault();
                var airport = await _context.Airports.ToArrayAsync();
                if (myHelico == null)
                {
                    return View(createUniqueRouteViewModel);
                }

                var startAirport = airport.Where(a => a.AirportId == createUniqueRouteViewModel.StartAirport).FirstOrDefault();
                var endAirport = airport.Where(a => a.AirportId == createUniqueRouteViewModel.EndAireport).FirstOrDefault();
                var myPilote = pilot.Where(p => p.Id == createUniqueRouteViewModel.IdPilote).FirstOrDefault();

                if (startAirport == null || endAirport == null || myPilote == null)
                {
                    return View(createUniqueRouteViewModel);
                }
                GeoCoordinate start = new GeoCoordinate() { Longitude = double.Parse(startAirport.Longitude, System.Globalization.CultureInfo.InvariantCulture), Latitude = double.Parse(startAirport.Latitude, System.Globalization.CultureInfo.InvariantCulture) };
                GeoCoordinate end = new GeoCoordinate() { Longitude = double.Parse(endAirport.Longitude, System.Globalization.CultureInfo.InvariantCulture), Latitude = double.Parse(endAirport.Latitude, System.Globalization.CultureInfo.InvariantCulture) };
                double kilometre = start.GetDistanceTo(end);
                kilometre = kilometre / 1000;
                kilometre = Math.Round(kilometre, 2);

                Route route = new Route() {
                    EndAirport = endAirport,
                    Helicopter = myHelico,
                    StartAirport = startAirport,
                    Pilote = myPilote,
                    PanifiedEnd = createUniqueRouteViewModel.EndFly,
                    PanifiedStart = createUniqueRouteViewModel.StartFly,
                    Place = createUniqueRouteViewModel.Size,
                    Type = "Ticket Unique",
                    Start = createUniqueRouteViewModel.DateFly,
                    Finish = false,
                    Kilometre = kilometre,
                    PlaceRemaining = createUniqueRouteViewModel.Size
                };


                _context.Add(new Message()
                {
                    Date = DateTime.Now,
                    Title = String.Format("Ajout du vol de {0}  à {1} ", route.StartAirport.Name, route.EndAirport.Name),
                    User = route.Pilote,
                    Text = String.Format("Bonjour, {0} nous vous annonçons que le vol pour {1} est un trajet unique et à destination de {2} a  été ajouté dans votre horaire.  Bien à vous JITC",
                   Environment.NewLine, route.StartAirport.Name, route.EndAirport.Name),

                });
                _context.Routes.Add(route);
                _context.SaveChanges();

                return RedirectToAction("ListOfRoute");
            }
            return View(createUniqueRouteViewModel);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> HebdoTicket()
        {
            HebdoRouteViewModel uniqueRouteViewModel = new HebdoRouteViewModel();
            uniqueRouteViewModel.Aiports = await _context.Airports.ToListAsync();
            return View(uniqueRouteViewModel);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HebdoTicket([Bind("StartAirport,EndAireport,StartFly,EndFly,DateFly,Day")] HebdoRouteViewModel hebdo)
        {

            if (ModelState.IsValid)
            {

                return RedirectToAction("CreateTicketHebdo", hebdo);
            }

            hebdo.Aiports = await _context.Airports.ToListAsync();
            return View(hebdo);
        }

        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateTicketHebdo(HebdoRouteViewModel? hebdo)
        {
            CreateHebdoRouteViewModel create = new();
            if (hebdo != null)
            {
                create.DateFly = hebdo.DateFly;
                create.EndFly = hebdo.EndFly;
                create.StartFly = hebdo.StartFly;
                create.EndAireport = hebdo.EndAireport;
                create.StartAirport = hebdo.StartAirport;
                create.Type = hebdo.Type;
                create.Day = hebdo.Day;
            }
            else
            {
                return RedirectToAction(nameof(ListOfRoute));
            }
            IList<DateTime> dateTimes = new List<DateTime>();

            var date = DateTime.Now.AddDays(1);
            for (int i = 0; date.Date <= create.DateFly.Date; i++)
            {
                if (date.DayOfWeek.ToString() == create.Day)
                {
                    dateTimes.Add(date);
                    date = date.AddDays(7);
                }
                else
                {
                    date = date.AddDays(1);
                }
            }


            IList<Route> sameDay = new List<Route>();

            foreach (var item in dateTimes)
            {
                var collection = _context.Routes.Include(r => r.Pilote).Include(r => r.Helicopter).Where(r => r.Start == item.Date).ToList();
                foreach (var route in collection)
                {
                    sameDay.Add(route);
                }

            }

            var routes = await _context.Routes.Include(r => r.Pilote).Include(r => r.Helicopter).ToArrayAsync();

            IList<Route> sameHour = new List<Route>();
            foreach (var route in sameDay)
            {

                sameHour.Add(route);

            }
            var helico = await _context.Helicopters.ToListAsync();
            var pilote = await _context.Roles.Where(r => r.Name == "Pilote").FirstAsync();
            var listOfPiloteId = await _context.UserRoles.Where(r => r.RoleId == pilote.Id).ToListAsync();
            IList<User> pilot = new List<User>();
            foreach (var item in listOfPiloteId)
            {
                pilot.Add(await _context.Users.Where(u => u.Id == item.UserId).FirstAsync());
            }
            foreach (var item in sameHour)
            {
                pilot.Remove(item.Pilote);
                helico.Remove(item.Helicopter);
            }
            create.Helicopters = helico;
            create.Pilote = pilot;
            return View(create);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTicketHebdo([Bind("StartAirport,EndAireport,StartFly,EndFly,DateFly,Type,IdPilote,IdHelicopter,Size,Day")] CreateHebdoRouteViewModel createUniqueRouteViewModel)
        {
            var helico = await _context.Helicopters.ToListAsync();
            var pilote = await _context.Roles.Where(r => r.Name == "Pilote").FirstAsync();
            var listOfPiloteId = await _context.UserRoles.Where(r => r.RoleId == pilote.Id).ToListAsync();
            IList<User> pilot = new List<User>();
            foreach (var item in listOfPiloteId)
            {
                pilot.Add(await _context.Users.Where(u => u.Id == item.UserId).FirstAsync());
            }
            var routes = await _context.Routes.Include(r => r.Pilote).Include(r => r.Helicopter).ToArrayAsync();
            var sameDay = routes.Where(r => r.Start.Date == createUniqueRouteViewModel.StartFly.Date);
            IList<Route> sameHour = new List<Route>();
            foreach (var route in sameDay)
            {
                if (createUniqueRouteViewModel.StartFly.Hour <= route.PanifiedStart.Hour && createUniqueRouteViewModel.EndFly.Hour >= route.PanifiedStart.Hour || createUniqueRouteViewModel.EndFly.Hour <= route.PanifiedStart.Hour && createUniqueRouteViewModel.EndFly.Hour >= route.PanifiedStart.Hour)
                {
                    sameHour.Add(route);
                }
            }
            foreach (var item in sameHour)
            {
                pilot.Remove(item.Pilote);
                helico.Remove(item.Helicopter);
            }
            createUniqueRouteViewModel.Helicopters = helico;
            createUniqueRouteViewModel.Pilote = pilot;


            if (ModelState.IsValid)
            {

                var myHelico = helico.Where(h => h.HelicopterId == createUniqueRouteViewModel.IdHelicopter).FirstOrDefault();
                var airport = await _context.Airports.ToArrayAsync();
                if (myHelico == null)
                {
                    return View(createUniqueRouteViewModel);
                }

                var startAirport = airport.Where(a => a.AirportId == createUniqueRouteViewModel.StartAirport).FirstOrDefault();
                var endAirport = airport.Where(a => a.AirportId == createUniqueRouteViewModel.EndAireport).FirstOrDefault();
                var myPilote = pilot.Where(p => p.Id == createUniqueRouteViewModel.IdPilote).FirstOrDefault();

                if (startAirport == null || endAirport == null || myPilote == null)
                {
                    return View(createUniqueRouteViewModel);
                }
                GeoCoordinate start = new GeoCoordinate()
                {
                    Longitude = double.Parse(startAirport.Longitude, System.Globalization.CultureInfo.InvariantCulture),
                    Latitude = double.Parse(startAirport.Latitude, System.Globalization.CultureInfo.InvariantCulture)
                };
                GeoCoordinate end = new GeoCoordinate()
                {
                    Longitude = double.Parse(endAirport.Longitude, System.Globalization.CultureInfo.InvariantCulture),
                    Latitude = double.Parse(endAirport.Latitude, System.Globalization.CultureInfo.InvariantCulture)
                };
                double kilometre = start.GetDistanceTo(end);
                kilometre = kilometre / 1000;
                kilometre = Math.Round(kilometre, 2);


                var time = DateTime.Now.AddDays(1);
                IList<DateTime> dateTimes = new List<DateTime>();

                var date = DateTime.Now.AddDays(1);
                for (int i = 0; date.Date <= createUniqueRouteViewModel.DateFly.Date; i++)
                {
                    if (date.DayOfWeek.ToString() == createUniqueRouteViewModel.Day)
                    {
                        dateTimes.Add(date);
                        date = date.AddDays(7);
                    }
                    else
                    {
                        date = date.AddDays(1);
                    }
                }

                foreach (var item in dateTimes)
                {
                    Route route = new Route()
                    {
                        EndAirport = endAirport,
                        Helicopter = myHelico,
                        StartAirport = startAirport,
                        Pilote = myPilote,
                        PanifiedEnd = createUniqueRouteViewModel.EndFly,
                        PanifiedStart = createUniqueRouteViewModel.StartFly,
                        Place = createUniqueRouteViewModel.Size,
                        Type = "Ticket Hebdomadaire",
                        Start = item,
                        Finish = false,
                        Kilometre = kilometre,
                        PlaceRemaining = createUniqueRouteViewModel.Size
                    };
                    _context.Routes.Add(route);
                }





                _context.Add(new Message()
                {
                    Date = DateTime.Now,
                    Title = String.Format("Ajout du vol de {0}  à {1} ", startAirport.Name, endAirport.Name),
                    User = myPilote,
                    Text = String.Format("Bonjour, {0} nous vous annonçons que le vol pour {1} est un trajet hebdomadaire et à destination de {2} a  été ajouté dans votre horaire.  Bien à vous JITC",
                   Environment.NewLine, startAirport.Name, endAirport.Name),

                });

                _context.SaveChanges();

                return RedirectToAction("ListOfRoute");
            }
            return View(createUniqueRouteViewModel);
        }




        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DayTicket()
        {
            UniqueRouteViewModel uniqueRouteViewModel = new UniqueRouteViewModel();
            uniqueRouteViewModel.Aiports = await _context.Airports.ToListAsync();
            return View(uniqueRouteViewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DayTicket([Bind("StartAirport,EndAireport,StartFly,EndFly,DateFly")] UniqueRouteViewModel uniqueRouteViewModel)
        {

            if (ModelState.IsValid)
            {

                return RedirectToAction("CreateTicketDay", uniqueRouteViewModel);
            }

            uniqueRouteViewModel.Aiports = await _context.Airports.ToListAsync();
            return View(uniqueRouteViewModel);
        }

        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateTicketDay(UniqueRouteViewModel? uniqueRouteViewModel)
        {
            CreateUniqueRouteViewModel createUniqueRouteViewModel = new CreateUniqueRouteViewModel();
            if (uniqueRouteViewModel != null)
            {
                createUniqueRouteViewModel.DateFly = uniqueRouteViewModel.DateFly;
                createUniqueRouteViewModel.EndFly = uniqueRouteViewModel.EndFly;
                createUniqueRouteViewModel.StartFly = uniqueRouteViewModel.StartFly;
                createUniqueRouteViewModel.EndAireport = uniqueRouteViewModel.EndAireport;
                createUniqueRouteViewModel.StartAirport = uniqueRouteViewModel.StartAirport;
                createUniqueRouteViewModel.Type = uniqueRouteViewModel.Type;
            }
            var routes = await _context.Routes.Include(r => r.Pilote).Include(r => r.Helicopter).ToArrayAsync();
            var sameDay = routes.Where(r => r.Start.Date <= createUniqueRouteViewModel.DateFly.Date);
            IList<Route> sameHour = new List<Route>();
            foreach (var route in sameDay)
            {

                sameHour.Add(route);

            }
            var helico = await _context.Helicopters.ToListAsync();
            var pilote = await _context.Roles.Where(r => r.Name == "Pilote").FirstAsync();
            var listOfPiloteId = await _context.UserRoles.Where(r => r.RoleId == pilote.Id).ToListAsync();
            IList<User> pilot = new List<User>();
            foreach (var item in listOfPiloteId)
            {
                pilot.Add(await _context.Users.Where(u => u.Id == item.UserId).FirstAsync());
            }
            foreach (var item in sameHour)
            {
                pilot.Remove(item.Pilote);
                helico.Remove(item.Helicopter);
            }
            createUniqueRouteViewModel.Helicopters = helico;
            createUniqueRouteViewModel.Pilote = pilot;
            return View(createUniqueRouteViewModel);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTicketDay([Bind("StartAirport,EndAireport,StartFly,EndFly,DateFly,Type,IdPilote,IdHelicopter,Size")] CreateUniqueRouteViewModel createUniqueRouteViewModel)
        {
            var helico = await _context.Helicopters.ToListAsync();
            var pilote = await _context.Roles.Where(r => r.Name == "Pilote").FirstAsync();
            var listOfPiloteId = await _context.UserRoles.Where(r => r.RoleId == pilote.Id).ToListAsync();
            IList<User> pilot = new List<User>();
            foreach (var item in listOfPiloteId)
            {
                pilot.Add(await _context.Users.Where(u => u.Id == item.UserId).FirstAsync());
            }
            var routes = await _context.Routes.Include(r => r.Pilote).Include(r => r.Helicopter).ToArrayAsync();
            var sameDay = routes.Where(r => r.Start.Date == createUniqueRouteViewModel.StartFly.Date);
            IList<Route> sameHour = new List<Route>();
            foreach (var route in sameDay)
            {
                if (createUniqueRouteViewModel.StartFly.Hour <= route.PanifiedStart.Hour && createUniqueRouteViewModel.EndFly.Hour >= route.PanifiedStart.Hour || createUniqueRouteViewModel.EndFly.Hour <= route.PanifiedStart.Hour && createUniqueRouteViewModel.EndFly.Hour >= route.PanifiedStart.Hour)
                {
                    sameHour.Add(route);
                }
            }
            foreach (var item in sameHour)
            {
                pilot.Remove(item.Pilote);
                helico.Remove(item.Helicopter);
            }
            createUniqueRouteViewModel.Helicopters = helico;
            createUniqueRouteViewModel.Pilote = pilot;


            if (ModelState.IsValid)
            {

                var myHelico = helico.Where(h => h.HelicopterId == createUniqueRouteViewModel.IdHelicopter).FirstOrDefault();
                var airport = await _context.Airports.ToArrayAsync();
                if (myHelico == null)
                {
                    return View(createUniqueRouteViewModel);
                }

                var startAirport = airport.Where(a => a.AirportId == createUniqueRouteViewModel.StartAirport).FirstOrDefault();
                var endAirport = airport.Where(a => a.AirportId == createUniqueRouteViewModel.EndAireport).FirstOrDefault();
                var myPilote = pilot.Where(p => p.Id == createUniqueRouteViewModel.IdPilote).FirstOrDefault();

                if (startAirport == null || endAirport == null || myPilote == null)
                {
                    return View(createUniqueRouteViewModel);
                }
                GeoCoordinate start = new GeoCoordinate() {
                    Longitude = double.Parse(startAirport.Longitude, System.Globalization.CultureInfo.InvariantCulture),
                    Latitude = double.Parse(startAirport.Latitude, System.Globalization.CultureInfo.InvariantCulture) };
                GeoCoordinate end = new GeoCoordinate() { Longitude = double.Parse(endAirport.Longitude, System.Globalization.CultureInfo.InvariantCulture),
                    Latitude = double.Parse(endAirport.Latitude, System.Globalization.CultureInfo.InvariantCulture) };
                double kilometre = start.GetDistanceTo(end);
                kilometre = kilometre / 1000;
                kilometre = Math.Round(kilometre, 2);


                var time = DateTime.Now.AddDays(1);

                for (int i = 0; !time.Date.Equals(createUniqueRouteViewModel.DateFly.Date); i++)
                {
                    Route route = new Route()
                    {
                        EndAirport = endAirport,
                        Helicopter = myHelico,
                        StartAirport = startAirport,
                        Pilote = myPilote,
                        PanifiedEnd = createUniqueRouteViewModel.EndFly,
                        PanifiedStart = createUniqueRouteViewModel.StartFly,
                        Place = createUniqueRouteViewModel.Size,
                        Type = "Ticket Quotidient",
                        Start = time,
                        Finish = false,
                        Kilometre = kilometre,
                        PlaceRemaining = createUniqueRouteViewModel.Size
                    };
                    _context.Routes.Add(route);
                    time = time.AddDays(1);


                }



                _context.Add(new Message()
                {
                    Date = DateTime.Now,
                    Title = String.Format("Ajout du vol de {0}  à {1} ", startAirport.Name, endAirport.Name),
                    User = myPilote,
                    Text = String.Format("Bonjour, {0} nous vous annonçons que le vol pour {1} est un trajet quotidien et à destination de {2} a  été ajouté dans votre horaire.  Bien à vous JITC",
                   Environment.NewLine, startAirport.Name, endAirport.Name),

                });

                _context.SaveChanges();

                return RedirectToAction("ListOfRoute");
            }
            return View(createUniqueRouteViewModel);
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Information(int id)
        {
            if (id < 0)
            {
                return RedirectToAction(nameof(ListOfRoute));
            }

            var route = await _context.Routes.Include(i => i.Pilote).Include(i => i.Helicopter).Include(r => r.EndAirport).Include(r => r.StartAirport)
                .FirstOrDefaultAsync(r => r.RouteId == id);

            if (route == null)
            {
                return RedirectToAction(nameof(ListOfRoute));
            }



            return View(route);

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id < 0)
            {
                return RedirectToAction(nameof(ListOfRoute));
            }
            var route = await _context.Routes.Include(i => i.Pilote).Include(i => i.Helicopter).Include(r => r.EndAirport).Include(r => r.StartAirport)
             .FirstOrDefaultAsync(r => r.RouteId == id);
            if (route == null)
            {
                return RedirectToAction(nameof(ListOfRoute));
            }
            return View(route);

        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            if (id < 0)
            {
                return RedirectToAction(nameof(ListOfRoute));
            }

            var route = await _context.Routes.Include(i => i.Pilote).Include(i => i.Helicopter).Include(r => r.EndAirport).Include(r => r.StartAirport)
          .FirstOrDefaultAsync(r => r.RouteId == id);

            if (route == null)
            {
                return RedirectToAction(nameof(ListOfRoute));
            }

            _context.Routes.Remove(route);


            var airport = await _context.Airports.ToListAsync();

           

            var ticket = await _context.Ticket.Include(r => r.User).Include(r => r.Route).Where(r => r.Route.RouteId == id).ToListAsync();
            _context.Add(new Message()
            {
                Date = DateTime.Now,
                Title = String.Format("Suppression du vol de {0}  à {1} ", route.StartAirport.Name, route.EndAirport.Name),
                User = route.Pilote,
                Text = String.Format("Bonjour, {0} nous vous annonçons que le vol pour {1} et  à destination de {2} a  été supprimé.  Bien à vous JITC",
                   Environment.NewLine, route.StartAirport.Name, route.EndAirport.Name),

            });

            foreach (var item in ticket)
            {
                _context.Add(new Message()
                {
                    Date = DateTime.Now,
                    Title = String.Format("Suppression du vol de {0}  à {1} ", route.StartAirport.Name, route.EndAirport.Name),
                    User = item.User,
                    Text = String.Format("Bonjour, {0} nous vous annonçons que le vol pour {1} et  à destination de {2} a  été supprimé.  Bien à vous JITC",
                  Environment.NewLine, route.StartAirport.Name, route.EndAirport.Name),

                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListOfRoute));
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var route = await _context.Routes.Include(r => r.Pilote).Include(r => r.EndAirport).Include(r => r.StartAirport).Where(r => r.RouteId == id).FirstOrDefaultAsync();

            if (route == null)
            {
                return RedirectToAction(nameof(ListOfRoute));
            }


            return View(new EditRouteViewModel() {
                Aiports = await _context.Airports.ToListAsync(),
                Id = id,
                DateFly = route.Start,
                EndAireport = route.EndAirport.AirportId,
                StartAirport = route.StartAirport.AirportId,
                EndFly = route.PanifiedEnd,
                StartFly = route.PanifiedStart,
                Type = route.Type,
            });

        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,StartAirport,EndAireport,StartFly,EndFly,DateFly,Type")] EditRouteViewModel edit)
        {
            if (ModelState.IsValid)
            {
                var route = await _context.Routes.Include(r => r.Pilote).Include(r => r.EndAirport).Include(r => r.StartAirport).Where(r => r.RouteId == edit.Id).FirstOrDefaultAsync();

                if (route == null)
                {
                    return RedirectToAction(nameof(ListOfRoute));
                }

                return RedirectToAction("EditConfirm", edit);
            }

            edit.Aiports = await _context.Airports.ToListAsync();
            return View(edit);

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditConfirm(EditRouteViewModel edit)
        {
            var route = await _context.Routes.Include(r => r.Pilote).Include(r=>r.Helicopter).Include(r => r.EndAirport).Include(r => r.StartAirport).Where(r => r.RouteId == edit.Id).FirstOrDefaultAsync();

            if (route == null)
            {
                return RedirectToAction(nameof(ListOfRoute));
            }
            ConfirmEditRouteViewModel confirm = new();
            if (edit != null)
            {
                confirm.DateFly = edit.DateFly;
                confirm.EndFly = edit.EndFly;
                confirm.StartFly = edit.StartFly;
                confirm.EndAireport = edit.EndAireport;
                confirm.StartAirport = edit.StartAirport;
                confirm.Type = edit.Type;
                confirm.Id = edit.Id;
            }
            var routes = await _context.Routes.Include(r => r.Pilote).Include(r => r.Helicopter).ToArrayAsync();
            var sameDay = routes.Where(r => r.Start.Date == confirm.DateFly.Date);
            IList<Route> sameHour = new List<Route>();
            foreach (var data in sameDay)
            {
                if (confirm.StartFly.Hour <= data.PanifiedStart.Hour && confirm.EndFly.Hour >= data.PanifiedStart.Hour || confirm.EndFly.Hour <= data.PanifiedStart.Hour && confirm.EndFly.Hour >= data.PanifiedStart.Hour)
                {
                    sameHour.Add(data);
                }
            }
            var helico = await _context.Helicopters.ToListAsync();
            var pilote = await _context.Roles.Where(r => r.Name == "Pilote").FirstAsync();
            var listOfPiloteId = await _context.UserRoles.Where(r => r.RoleId == pilote.Id).ToListAsync();
            IList<User> pilot = new List<User>();
            foreach (var item in listOfPiloteId)
            {
                pilot.Add(await _context.Users.Where(u => u.Id == item.UserId).FirstAsync());
            }
            foreach (var item in sameHour)
            {
                if (item.RouteId != route.RouteId)
                {
                    pilot.Remove(item.Pilote);
                    helico.Remove(item.Helicopter);
                }
               
            }
            confirm.Helicopters = helico;
            confirm.Pilote = pilot;
            confirm.IdHelicopter = route.Helicopter.HelicopterId;
            confirm.IdPilote = route.Pilote.Id;
            confirm.Size = route.Place;

            return View(confirm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConfirm([Bind("Id,StartAirport,EndAireport,StartFly,EndFly,DateFly,Type,IdPilote,IdHelicopter,Size")] ConfirmEditRouteViewModel edit)
        {

            var route = await _context.Routes.Include(r => r.Pilote).Include(r => r.Helicopter).Include(r => r.EndAirport).Include(r => r.StartAirport).Where(r => r.RouteId == edit.Id).FirstOrDefaultAsync();


            var helico = await _context.Helicopters.ToListAsync();
            var pilote = await _context.Roles.Where(r => r.Name == "Pilote").FirstAsync();
            var listOfPiloteId = await _context.UserRoles.Where(r => r.RoleId == pilote.Id).ToListAsync();
            IList<User> pilot = new List<User>();
            foreach (var item in listOfPiloteId)
            {
                pilot.Add(await _context.Users.Where(u => u.Id == item.UserId).FirstAsync());
            }
            var routes = await _context.Routes.Include(r => r.Pilote).Include(r => r.Helicopter).ToArrayAsync();
            var sameDay = routes.Where(r => r.Start.Date == edit.StartFly.Date);
            IList<Route> sameHour = new List<Route>();
            foreach (var data in sameDay)
            {
                if (edit.StartFly.Hour <= data.PanifiedStart.Hour && edit.EndFly.Hour >= data.PanifiedStart.Hour || edit.EndFly.Hour <= data.PanifiedStart.Hour && edit.EndFly.Hour >= data.PanifiedStart.Hour)
                {
                    sameHour.Add(data);
                }
            }
            foreach (var item in sameHour)
            {
                if (item.RouteId != route.RouteId)
                {
                    pilot.Remove(item.Pilote);
                    helico.Remove(item.Helicopter);
                }
            }
            edit.Helicopters = helico;
            edit.Pilote = pilot;


            if (ModelState.IsValid)
            {

                var myHelico = helico.Where(h => h.HelicopterId == edit.IdHelicopter).FirstOrDefault();
                var airport = await _context.Airports.ToArrayAsync();
                if (myHelico == null)
                {
                    return View(edit);
                }

                var startAirport = airport.Where(a => a.AirportId == edit.StartAirport).FirstOrDefault();
                var endAirport = airport.Where(a => a.AirportId == edit.EndAireport).FirstOrDefault();
                var myPilote = pilot.Where(p => p.Id == edit.IdPilote).FirstOrDefault();

                if (startAirport == null || endAirport == null || myPilote == null)
                {
                    return View(edit);
                }
                GeoCoordinate start = new GeoCoordinate() { Longitude = double.Parse(startAirport.Longitude, System.Globalization.CultureInfo.InvariantCulture), Latitude = double.Parse(startAirport.Latitude, System.Globalization.CultureInfo.InvariantCulture) };
                GeoCoordinate end = new GeoCoordinate() { Longitude = double.Parse(endAirport.Longitude, System.Globalization.CultureInfo.InvariantCulture), Latitude = double.Parse(endAirport.Latitude, System.Globalization.CultureInfo.InvariantCulture) };
                double kilometre = start.GetDistanceTo(end);
                kilometre = kilometre / 1000;
                kilometre = Math.Round(kilometre, 2);




                var listReservation = _context.Ticket.Include(t => t.Route).Include(t => t.User).Where(t => t.Route.RouteId == edit.Id);

                foreach (var item in listReservation)
                {
                    _context.Add(new Message() {
                        Date = DateTime.Now,
                        Title = String.Format("Modification du vol de {0}  à {1} ", route.StartAirport.Name, route.EndAirport.Name),
                        User = route.Pilote,
                        Text = String.Format("Bonjour, {0} nous vous annonçons que le vol pour {1} est un trajet unique et  à destination de {2} a été modifié.  Bien à vous JITC",
                   Environment.NewLine, route.StartAirport.Name, route.EndAirport.Name),

                    });
                }

                route.EndAirport = endAirport;
                route.Helicopter = myHelico;
                route.StartAirport = startAirport;
                route.Pilote = myPilote;
                route.PanifiedEnd = edit.EndFly;
                route.PanifiedStart = edit.StartFly;
                route.Place = edit.Size;
                route.Type = edit.Type;
                route.Start = edit.DateFly;
                route.Finish = false;
                route.Kilometre = kilometre;
                route.PlaceRemaining = edit.Size;





                _context.SaveChanges();

                return RedirectToAction("ListOfRoute");
            }


            return View(edit);
        }



        /// partie utilisateur

        [Authorize]
        public async Task<IActionResult> Index(string start = "", string end ="", DateTime time = default)
        {


            var user = await _context.Users.Include(u => u.Messages).Where(u => u.Id == _userManager.GetUserId(HttpContext.User)).FirstOrDefaultAsync();
            var notification = user.Messages.Where(m => m.IsRead == false);
            IQueryable<Route> currentFly = null;

            if (!string.IsNullOrEmpty(start))
            {

                if (!string.IsNullOrEmpty(end))
                {
                    if (time != default)
                    {
                        ViewData["start"] = start;
                        ViewData["end"] = end;
                        ViewData["time"] = time.ToString("yyyy-MM-dd");
                        currentFly = _context.Routes.Include(i => i.Pilote).Include(i => i.Helicopter).Include(r => r.StartAirport).Include(r => r.EndAirport).Include(r => r.Helicopter).Where(r => r.StartAirport.Name.Contains(start) && r.EndAirport.Name.Contains(end) && r.Start.Date == time.Date );
                    }
                    else
                    {
                        ViewData["start"] = start;
                        ViewData["end"] = end;
                        currentFly = _context.Routes.Include(i => i.Pilote).Include(i => i.Helicopter).Include(r => r.StartAirport).Include(r => r.EndAirport).Include(r => r.Helicopter).Where(r => r.StartAirport.Name.Contains(start) && r.EndAirport.Name.Contains(end) );
                    }
                }
                else
                {
                    if (time != default)
                    {
                        ViewData["start"] = start;

                        ViewData["time"] = time.ToString("yyyy-MM-dd");
                        currentFly = _context.Routes.Include(i => i.Pilote).Include(i => i.Helicopter).Include(r => r.StartAirport).Include(r => r.EndAirport).Include(r => r.Helicopter).Where(r => r.StartAirport.Name.Contains(start) && r.Start.Date == time.Date );
                    }
                    else
                    {
                        ViewData["start"] = start;
                        currentFly = _context.Routes.Include(i => i.Pilote).Include(i => i.Helicopter).Include(r => r.StartAirport).Include(r => r.EndAirport).Include(r => r.Helicopter).Where(r => r.StartAirport.Name.Contains(start) );
                    }
                }

            }
            else if (!string.IsNullOrEmpty(end))
            {

                if (!string.IsNullOrEmpty(start))
                {
                    if (time != default)
                    {
                        ViewData["start"] = start;
                        ViewData["end"] = end;
                        ViewData["time"] = time;
                        currentFly = _context.Routes.Include(i => i.Pilote).Include(i => i.Helicopter).Include(r => r.StartAirport).Include(r => r.EndAirport).Include(r => r.Helicopter).Where(r => r.StartAirport.Name.Contains(start) && r.EndAirport.Name.Contains(end) && r.Start.Date == time.Date );
                    }
                    else
                    {
                        ViewData["start"] = start;
                        ViewData["end"] = end;

                        currentFly = _context.Routes.Include(i => i.Pilote).Include(i => i.Helicopter).Include(r => r.StartAirport).Include(r => r.EndAirport).Include(r => r.Helicopter).Where(r => r.StartAirport.Name.Contains(start) && r.EndAirport.Name.Contains(end) );
                    }
                }
                else
                {
                    if (time != default)
                    {

                        ViewData["end"] = end;
                        ViewData["time"] = time.ToString("yyyy-MM-dd");
                        currentFly = _context.Routes.Include(i => i.Pilote).Include(i => i.Helicopter).Include(r => r.StartAirport).Include(r => r.EndAirport).Include(r => r.Helicopter).Where(r => r.EndAirport.Name.Contains(end) && r.Start.Date == time.Date );
                    }
                    else
                    {

                        ViewData["end"] = end;

                        currentFly = _context.Routes.Include(i => i.Pilote).Include(i => i.Helicopter).Include(r => r.StartAirport).Include(r => r.EndAirport).Include(r => r.Helicopter).Where(r => r.EndAirport.Name.Contains(end) );
                    }
                }
            }
            else if (time != default) 
            {
                if (!string.IsNullOrEmpty(start))
                {
                    if (!string.IsNullOrEmpty(end))
                    {
                        ViewData["start"] = start;
                        ViewData["end"] = end;
                        ViewData["time"] = time.ToString("yyyy-MM-dd");
                        currentFly = _context.Routes.Include(i => i.Pilote).Include(i => i.Helicopter).Include(r => r.StartAirport).Include(r => r.EndAirport).Include(r => r.Helicopter).Where(r => r.StartAirport.Name.Contains(start) && r.EndAirport.Name.Contains(end) && r.Start.Date == time.Date );
                    }
                    else
                    {
                        ViewData["start"] = start;

                        ViewData["time"] = time.ToString("yyyy-MM-dd");
                        currentFly = _context.Routes.Include(i => i.Pilote).Include(i => i.Helicopter).Include(r => r.StartAirport).Include(r => r.EndAirport).Include(r => r.Helicopter).Where(r => r.StartAirport.Name.Contains(start)  && r.Start.Date == time.Date );
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(end))
                    {

                        ViewData["end"] = end;
                        ViewData["time"] = time.ToString("yyyy-MM-dd");
                        currentFly = _context.Routes.Include(i => i.Pilote).Include(i => i.Helicopter).Include(r => r.StartAirport).Include(r => r.EndAirport).Include(r => r.Helicopter).Where(r =>   r.EndAirport.Name.Contains(end) && r.Start.Date == time.Date );
                    }
                    else
                    {
                        ViewData["time"] = time.ToString("yyyy-MM-dd");
                        currentFly = _context.Routes.Include(i => i.Pilote).Include(i => i.Helicopter).Include(r => r.StartAirport).Include(r => r.EndAirport).Include(r => r.Helicopter).Where(r =>   r.Start.Date == time.Date ); ;
                    }
                }


            }

            if (string.IsNullOrEmpty(start) && string.IsNullOrEmpty(end) && time == default )
            {
                var route = _context.Routes.Include(i => i.Pilote).Include(i => i.Helicopter).Include(r => r.StartAirport).Include(r => r.EndAirport).Include(r => r.Helicopter);

                currentFly =  route;


               
            }

            return View(new ListOfCurrentRoute()
            {
                Notification = notification.Count(),
                CurrentRoute = currentFly.Where(r => r.Finish == false && r.Start.Date >= DateTime.Now.Date).ToList()


            }); 
        }



        [Produces("application/json")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FindAirportStart() 
        {
            string term = HttpContext.Request.Query["startAirport"].ToString();
            var airport = await _context.Routes.Include(r => r.StartAirport).Where(r => r.StartAirport.Name.Contains(term) && r.Finish == false && r.Start.Date >= DateTime.Now.Date).Select(r => r.StartAirport.Name).ToListAsync();
            return Ok(airport.Distinct().ToList());
        }

        [Produces("application/json")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FindAirportEnd()
        {
            string term = HttpContext.Request.Query["startAirport"].ToString();
            var airport = await _context.Routes.Include(r => r.EndAirport).Where(r => r.EndAirport.Name.Contains(term) && r.Finish == false && r.Start.Date >= DateTime.Now.Date).Select(r => r.EndAirport.Name).ToListAsync();
            return Ok(airport.Distinct().ToList());
        }




        // partie pilote

        [Authorize( Roles = "Pilote")]
        public async Task<IActionResult> RoutePilote()
        {
            var user = await _context.Users.Include(u => u.Messages).Where(u => u.Id == _userManager.GetUserId(HttpContext.User)).FirstOrDefaultAsync();
            var notification = user.Messages.Where(m => m.IsRead == false);
            var route = await _context.Routes.Include(i => i.Pilote).Include(i => i.Helicopter).Include(r => r.StartAirport).Include(r => r.EndAirport).Where(r => r.Pilote.Id == user.Id && r.Finish == false ).ToListAsync();

            route = route.OrderBy(r => r.Start.Date).ToList();


            return View( new ListOfCurrentRoute() { 
                CurrentRoute = route,
                Notification = notification.Count(),
            });
        }


       


    }
}
