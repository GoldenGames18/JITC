using JITC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JITC.Controllers
{
    public class AirportController : Controller
    {

        private readonly JITCSDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AirportController(JITCSDbContext context, SignInManager<User> signInManager, UserManager<User> UserManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = UserManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListOfAirport()
        {
            return View( await _context.Airports.ToListAsync());
        }


        [Authorize(Roles = "Admin")]
        public IActionResult AddAirport()
        {
            return View(new Airport());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAirport([Bind("Name,Latitude,Longitude")] Airport airport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(airport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListOfAirport));
            }

            return View(airport);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditAirport(int id) 
        {
            if (id < 0)
            {
                return RedirectToAction(nameof(ListOfAirport));
            }

            var helicopter = await _context.Airports
                .FirstOrDefaultAsync(u => u.AirportId == id);

            if (helicopter == null)
            {
                return RedirectToAction(nameof(ListOfAirport));
            }

            return View(helicopter);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAirport([Bind("AirportId,Name,Latitude,Longitude")] Airport airport) 
        {
            if (airport.AirportId < 0)
            {
                return RedirectToAction(nameof(ListOfAirport));
            }

            var currentAirport = await _context.Airports
                .FirstOrDefaultAsync(u => u.AirportId == airport.AirportId);

            if (currentAirport == null)
            {
                return RedirectToAction(nameof(ListOfAirport));
            }

            if (ModelState.IsValid)
            {

                currentAirport.Longitude = airport.Longitude;
                currentAirport.Latitude = airport.Latitude;
                currentAirport.Name = airport.Name;

                _context.Update(currentAirport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListOfAirport));
            }

            return View(airport);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id < 0)
            {
                return RedirectToAction(nameof(ListOfAirport));
            }

            var airport = await _context.Airports
                .FirstOrDefaultAsync(u => u.AirportId == id);

            if (airport == null)
            {
                return RedirectToAction(nameof(ListOfAirport));
            }

            return View(airport);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            if (id < 0)
            {
                return RedirectToAction(nameof(ListOfAirport));
            }

            var airport = await _context.Airports.FirstOrDefaultAsync(u => u.AirportId == id);

            if (airport == null)
            {
                return RedirectToAction(nameof(ListOfAirport));
            }

            _context.Airports.Remove(airport);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListOfAirport));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Information(int id)
        {
            if (id < 0)
            {
                return RedirectToAction(nameof(ListOfAirport));
            }

            var airport = await _context.Airports
                .FirstOrDefaultAsync(u => u.AirportId == id);

            if (airport == null)
            {
                return RedirectToAction(nameof(ListOfAirport));
            }

            return View(airport);
        }


    }
}
