using JITC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JITC.Controllers
{
    public class MessageController : Controller
    {

        private readonly JITCSDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public MessageController(JITCSDbContext context, SignInManager<User> signInManager, UserManager<User> UserManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = UserManager;
        }


        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _context.Users.Include(u => u.Messages).Where(u => u.Id == _userManager.GetUserId(HttpContext.User)).FirstOrDefaultAsync();
            return View( user.Messages.OrderByDescending(m => m.Date));
        }

        [Authorize]
        public async Task<IActionResult> ReadMessage(int id)
        {
            if (id < 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var user =  await _context.Users.Include(u => u.Messages).Where(u => u.Id == _userManager.GetUserId(HttpContext.User)).FirstOrDefaultAsync();
            var message = user.Messages.Where(m => m.IdMessage == id).FirstOrDefault();


            if (message == null)
            {
                return RedirectToAction(nameof(Index));
            }

            message.IsRead = true;
            _context.SaveChanges();
            
            return View(message);
        }


    }
}
