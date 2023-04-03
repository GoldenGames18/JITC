using JITC.Models;
using JITC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JITC.Controllers
{
    public class DashboardController : Controller
    {


        private readonly JITCSDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public DashboardController(JITCSDbContext context, SignInManager<User> signInManager, UserManager<User> UserManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = UserManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var dashboard =  new DashboardViewModels();

            //number of helicopter
            var helico = await _context.Helicopters.ToListAsync();
            dashboard.NumberHelyco = helico.Count;

            //number of pilote and list of pilote
            var pilote =  await _context.Roles.Where(r => r.Name == "Pilote").FirstAsync();
            var listOfPiloteId = await _context.UserRoles.Where(r => r.RoleId == pilote.Id).ToListAsync();

            IList<User> pilot = new List<User>();

            foreach (var item in listOfPiloteId)
            {
                pilot.Add(await _context.Users.Where(u => u.Id == item.UserId).FirstAsync());
            }

            dashboard.NumberPilote = pilot.Count;
            dashboard.Pilote = pilot;

            //number of user
            var allUser = await _context.Users.ToListAsync();
            dashboard.NumberUser = allUser.Count - pilot.Count;


            //number of route
            var route = await _context.Routes.ToListAsync();
            dashboard.NumberRoute = route.Count;

            return View( dashboard);
        }


        

    }
}
