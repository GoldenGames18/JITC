using JITC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JITC.Controllers
{
    public class HelicopterController : Controller
    {
        private readonly JITCSDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public HelicopterController(JITCSDbContext context, SignInManager<User> signInManager, UserManager<User> UserManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = UserManager;
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListOfHelicopter() 
        {
            var helicopteres =  _context.Helicopters.ToList();
            return View(helicopteres);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddHelicopter() 
        {
            return View(new Helicopter());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddHelicopter([Bind("HelicopterName,Size,Speed,Engine")] Helicopter helicopter)
        {
            if (ModelState.IsValid)
            {
                _context.Add(helicopter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListOfHelicopter));
            }

            return View(helicopter);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditHelicopter(int id)
        {
            if(id < 0)
            {
                return RedirectToAction(nameof(ListOfHelicopter));
            }

            var helicopter = await _context.Helicopters
                .FirstOrDefaultAsync(u => u.HelicopterId == id);

            if (helicopter == null)
            {
                return RedirectToAction(nameof(ListOfHelicopter));
            }

            return View(helicopter);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHelicopter([Bind("HelicopterId,HelicopterName,Size,Speed,Engine")] Helicopter helicopter)
        {
            var currentHelicopter = await _context.Helicopters.FirstOrDefaultAsync(u => u.HelicopterId == helicopter.HelicopterId);
            if (currentHelicopter == null)
            {
                return View(currentHelicopter);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    currentHelicopter.HelicopterName = helicopter.HelicopterName;
                    currentHelicopter.Speed = helicopter.Speed;
                    currentHelicopter.Engine = helicopter.Engine;
                    currentHelicopter.Size = helicopter.Size;
       
                    _context.Update(currentHelicopter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return View(helicopter);
                }
                return RedirectToAction(nameof(ListOfHelicopter));
            }
            return View(helicopter);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id < 0)
            {
                return RedirectToAction(nameof(ListOfHelicopter));
            }

            var helicopter = await _context.Helicopters
                .FirstOrDefaultAsync(u => u.HelicopterId == id);

            if (helicopter == null)
            {
                return RedirectToAction(nameof(ListOfHelicopter));
            }

            return View(helicopter);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            if (id < 0)
            {
                return RedirectToAction(nameof(ListOfHelicopter));
            }

            var helicopter = await _context.Helicopters.FirstOrDefaultAsync(u => u.HelicopterId == id);

            if (helicopter == null)
            {
                return RedirectToAction(nameof(ListOfHelicopter));
            }

            _context.Helicopters.Remove(helicopter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListOfHelicopter));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Garage() 
        {

            return View(_context.Helicopters.Where(h => h.Fly == 5 ));
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ResetFly(int id)
        {
            if (id < 0 )
            {
                return RedirectToAction(nameof(Garage));
            }
            var helicopter = await _context.Helicopters.FirstOrDefaultAsync(u => u.HelicopterId == id);
            if (helicopter == null)
            {
                return RedirectToAction(nameof(ListOfHelicopter));
            }
            if (helicopter.Fly < 5)
            {
                return RedirectToAction(nameof(Garage));
            }
            helicopter.ResetFly();
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Garage));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Information(int id)
        {
            if (id < 0)
            {
                return RedirectToAction(nameof(ListOfHelicopter));
            }

            var helicopter = await _context.Helicopters
                .FirstOrDefaultAsync(u => u.HelicopterId == id);

            if (helicopter == null)
            {
                return RedirectToAction(nameof(ListOfHelicopter));
            }

            return View(helicopter);
        }



    }
}
