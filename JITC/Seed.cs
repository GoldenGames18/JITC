using JITC.Models;
using Microsoft.AspNetCore.Identity;
using System;
using Route = JITC.Models.Route;

public class Seed
{
		public  static void Start(RoleManager<IdentityRole> _roleManager, UserManager<User> _userManager, JITCSDbContext _context) 
		{
			CreateRole(_roleManager);
			CreateUser(_userManager, _context);
		}

		private static void CreateRole(RoleManager<IdentityRole> _roleManager) 
		{
			IdentityRole role = _roleManager.FindByIdAsync("Admin").Result;
			if (role == null)
			{
				var add = _roleManager.CreateAsync(new IdentityRole("Admin")).Result;
				add = _roleManager.CreateAsync(new IdentityRole("Pilote")).Result;
				add = _roleManager.CreateAsync(new IdentityRole("Client")).Result;

			}
		}

		private static void CreateUser(UserManager<User> _userManager, JITCSDbContext _context)
		{
			var role = _context.Users.ToList(); ;
			if (role.Count == 0)
			{
			    User user1 = new User() {
					Name = "Vincent", 
					LastName = "Nicolas",
					Email = "nico@nico.nico",
					UserName = "nico@nico.nico", 
					Path = "~/img/user.png", 
	
				};
			    User user2 = new User()
				{
					Name = "Pierro",
					LastName = "Lustucru",
					Email = "pepito@pepito.p",
					UserName = "pepito@pepito.p",
					Path = "~/img/user.png",

				};
			    User user3 = new User() {
					Name = "User",
					LastName = "user",
					Email = "user@user.user",
					UserName = "user@user.user",
					Path = "~/img/user.png",
				};
			    User admin = new User(){
					Name = "Ney",
					LastName = "Mo",
					Email = "M.Ney@jitc.com",
					UserName = "M.Ney@jitc.com",
					Path = "~/img/user.png",

				};
			    User  pilote1 = new User()
				{
					Name = "Balav",
					LastName = "Danièle",
					Email = "D.Balav@jitc.com",
					UserName = "D.Balav@jitc.com",
					Path = "~/img/user.png",

				};
			    User pilote2 = new User()
				{
					Name = "Sabine ",
					LastName = "Thierry",
					Email = "T.Sabine@jitc.com",
					UserName = "T.Sabine@jitc.com",
					Path = "~/img/user.png",

				};
				User pilote3 = new User()
				{
					Name = "Coptère",
					LastName = "Eli",
					Email = "E.Coptere@jitc.com",
					UserName = "E.Coptere@jitc.com",
					Path = "~/img/user.png",

				};


				var add = _userManager.CreateAsync(user1, "wStN!>&78Pu4!S6q4g&S").Result;
			    add = _userManager.CreateAsync(user2, "XGEayA5>t4m!9Yy9-!9*").Result;
			    add = _userManager.CreateAsync(user3, "X45nuB=4Y$4j&)a/XwQ6").Result;
			    add = _userManager.CreateAsync(admin, "-g2?Zh<}3Lb|d,3rPcBSaYe6(%59qR;)E8N7F45/H9bc:uvD64").Result;
			    add = _userManager.CreateAsync(pilote1, "aQS#FL9?z52s7=s").Result;
			    add = _userManager.CreateAsync(pilote2, "ZC^Uy6c@6b6i6R{").Result;
			    add = _userManager.CreateAsync(pilote3, "559&!Zmzxb!YP2Z").Result;


				var test = _userManager.AddToRoleAsync(user1, "Client").Result;
				test = _userManager.AddToRoleAsync(user2, "Client").Result;
				test = _userManager.AddToRoleAsync(user3, "Client").Result;
				test = _userManager.AddToRoleAsync(admin, "Admin").Result;
				test = _userManager.AddToRoleAsync(pilote1, "Pilote").Result;
				test = _userManager.AddToRoleAsync(pilote2, "Pilote").Result;
				test = _userManager.AddToRoleAsync(pilote3, "Pilote").Result;

			var pastRoute1 = new Route()
			{
				Pilote = pilote1,
				Finish = true,
				PanifiedStart = new DateTime(2022, 7, 1, 10, 0, 0),
				PanifiedEnd = new DateTime(2022, 7, 1, 11, 0, 0),
				Helicopter = _context.Helicopters.Where(h => h.HelicopterName == "SwilaCoptère").FirstOrDefault(),
				StartAirport = _context.Airports.Where(h => h.Name == "Liège").FirstOrDefault(),
				EndAirport = _context.Airports.Where(h => h.Name == "Charleroi").FirstOrDefault(),
				Kilometre = 74.7,
				Place = 3,
				Start = new DateTime(2022, 8, 4),
				PlaceRemaining = 0,
				Type = "Trajet Unique"

			};

			var report1 = new Report()
			{
				Start = new DateTime(2022, 7, 1, 10, 0, 0),
				End = new DateTime(2022, 7, 1, 11, 0, 0),
				Message = "Rien à signaler",
				Route =  pastRoute1
			};

			var ticketPast1 = new Ticket()
			{
				Date =  new DateTime(2022, 8, 1),
				Place = 3,
				Route = pastRoute1,
				Type = "terminé",
				User = user3
			};


			var pastRoute2 = new Route()
			{
				Pilote = pilote2,
				Finish = true,
				PanifiedStart = new DateTime(2022, 7, 1, 19, 0, 0),
				PanifiedEnd = new DateTime(2022, 7, 1, 22, 0, 0),
				Helicopter = _context.Helicopters.Where(h => h.HelicopterName == "Robinson R44 Raven II").FirstOrDefault(),
				StartAirport = _context.Airports.Where(h => h.Name == "Bruxelles").FirstOrDefault(),
				EndAirport = _context.Airports.Where(h => h.Name == "Charleroi").FirstOrDefault(),
				Kilometre = 54.64,
				Place = 3,
				Start = new DateTime(2022, 8, 1),
				PlaceRemaining = 0,
				Type = "Trajet Unique"

			};

			var report2 = new Report()
			{
				Start = new DateTime(2022, 7, 1, 19, 0, 0),
				End = new DateTime(2022, 7, 1, 22, 0, 0),
				Message = "Rien à signaler",
				Route = pastRoute2
			};

			var ticketPast2 = new Ticket()
			{
				Date = new DateTime(2022, 7, 20),
				Place = 3,
				Route = pastRoute2,
				Type = "terminé",
				User = user2
			};



			var futurRoute = new Route()
			{
				Pilote = pilote3,
				Finish = false,
				PanifiedStart = new DateTime(2022, 7, 1, 19, 0, 0),
				PanifiedEnd = new DateTime(2022, 7, 1, 22, 0, 0),
				Helicopter = _context.Helicopters.Where(h => h.HelicopterName == "Robinson R44 Raven II").FirstOrDefault(),
				StartAirport = _context.Airports.Where(h => h.Name == "Bruxelles").FirstOrDefault(),
				EndAirport = _context.Airports.Where(h => h.Name == "Charleroi").FirstOrDefault(),
				Kilometre = 54.64,
				Place = 3,
				Start = new DateTime(2022, 8, 20),
				PlaceRemaining = 1,
				Type = "Trajet Unique"

			};

			var futurRoute2 = new Route()
			{
				Pilote = pilote2,
				Finish = false,
				PanifiedStart = new DateTime(2022, 7, 1, 19, 0, 0),
				PanifiedEnd = new DateTime(2022, 7, 1, 22, 0, 0),
				Helicopter = _context.Helicopters.Where(h => h.HelicopterName == "Robinson R44 Raven II").FirstOrDefault(),
				StartAirport = _context.Airports.Where(h => h.Name == "Bruxelles").FirstOrDefault(),
				EndAirport = _context.Airports.Where(h => h.Name == "Charleroi").FirstOrDefault(),
				Kilometre = 54.64,
				Place = 3,
				Start = new DateTime(2022, 8, 16),
				PlaceRemaining = 2,
				Type = "Trajet Unique"
			};


			var futurRoute3 = new Route()
			{
				Pilote = pilote3,
				Finish = false,
				PanifiedStart = new DateTime(2022, 7, 1, 19, 0, 0),
				PanifiedEnd = new DateTime(2022, 7, 1, 22, 0, 0),
				Helicopter = _context.Helicopters.Where(h => h.HelicopterName == "Robinson R44 Raven II").FirstOrDefault(),
				StartAirport = _context.Airports.Where(h => h.Name == "Oostende").FirstOrDefault(),
				EndAirport = _context.Airports.Where(h => h.Name == "Charleroi").FirstOrDefault(),
				Kilometre = 54.64,
				Place = 3,
				Start = new DateTime(2022, 8, 18),
				PlaceRemaining = 0,
				Type = "Trajet Unique"
			};


			var futurTicket1= new Ticket() { 
				Date = new DateTime(2022, 8, 4),
				Place = 2,
				Route = futurRoute,
				Type = "réserver",
				User = user3

			};


			var futurTicket2= new Ticket() {
				Date = new DateTime(2022, 8, 3),
				Place = 1,
				Route = futurRoute2,
				Type = "réserver",
				User = user1
			};
			var futurTicket3= new Ticket() {
				Date = new DateTime(2022, 8, 1),
				Place = 3,
				Route = futurRoute3,
				Type = "réserver",
				User = user2
			};


			_context.Routes.Add(pastRoute1);
			_context.Routes.Add(pastRoute2);
			_context.Routes.Add(futurRoute);
			_context.Routes.Add(futurRoute2);
			_context.Routes.Add(futurRoute3);

			_context.Add(report2);
			_context.Add(report1);

			_context.Ticket.Add(futurTicket1);
			_context.Ticket.Add(futurTicket2);
			_context.Ticket.Add(futurTicket3);
			_context.Ticket.Add(ticketPast2);
			_context.Ticket.Add(ticketPast1);

			 _context.SaveChanges();

		}
	}


}
