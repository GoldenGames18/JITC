using JITC.Models;
using JITC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JITC.Controllers
{
    public class StatistiqueController : Controller
    {


        private readonly JITCSDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public StatistiqueController(JITCSDbContext context, SignInManager<User> signInManager, UserManager<User> UserManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = UserManager;
        }


        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Index()
        {
            var helico = _context.Helicopters.ToList();
            string[] nameHelico = new string[helico.Count  ];
            double[] valueHelico = new double[helico.Count];
            
            for (int i = 0; i < helico.Count(); i++) 
            {
                var max = 0;
                var min = 0;
                var route = await _context.Routes.Include(r => r.Helicopter).Where(r => r.Helicopter.HelicopterId == helico[i].HelicopterId).Where(r => r.Finish == true).ToListAsync();
                foreach (var item in route)
                {
                    max+= item.Place;
                    min+= item.Place - item.PlaceRemaining;
                }

                if (max == 0 || min == 0)
                {
                    valueHelico[i] = 0;
                }
                else
                {
                    double data = 0.0;
                     data =  ((double)min / (double)max) ;
                    valueHelico[i] = Math.Round( data * 100 , 1);
                }
                
                nameHelico[i] = helico[i].HelicopterName;


              
            }

            return View( new StatViewModel { Name = nameHelico, Value = valueHelico} );
        }
    }
}
