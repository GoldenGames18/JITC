using JITC.Models;
using JITC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace JITC.Controllers
{
    public class HomeController : Controller
    {
        private readonly JITCSDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;




        public HomeController(JITCSDbContext context, SignInManager<User> signInManager, UserManager<User> UserManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = UserManager;
        }

       

        public async Task<IActionResult> Index()
        {

            if (_signInManager.IsSignedIn(User))
            {
                var user = await _context.Users.Include(u => u.Messages).Where(u => u.Id == _userManager.GetUserId(HttpContext.User)).FirstOrDefaultAsync();
                var notification = user.Messages.Where(m => m.IsRead == false);
                return View( new NotificationViewModel() { Notification = notification.Count() });
            }
            else 
            {
                return View();
            }

            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


       
    }
}