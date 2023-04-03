using JITC.Models;
using JITC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JITC.Controllers
{
    public class UserController : Controller
    {

        private readonly JITCSDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public UserController(JITCSDbContext context, SignInManager<User> signInManager, UserManager<User> UserManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = UserManager;
        }

        
        [Authorize]
        public async Task<IActionResult> Index()
        {
            
            var user = await _context.Users.Include(u => u.Messages).Where(u => u.Id == _userManager.GetUserId(HttpContext.User)).FirstOrDefaultAsync();
            var idRoleUser = _context.UserRoles.Where(r => r.UserId == user.Id).First().RoleId;
            var role = _context.Roles.Where(r => r.Id == idRoleUser).First().Name;
            var notification = user.Messages.Where(m => m.IsRead == false);


            return View(new UserViewModels() { Name = user.Name, Email = user.Email, LastName = user.LastName, Path = user.Path, Role = role , Notification = notification.Count()}) ;
        }

        [Authorize]
    
        public async Task<IActionResult> EditUser() 
        {
            var user = await _context.Users.Include(u => u.Messages).Where(u => u.Id == _userManager.GetUserId(HttpContext.User)).FirstOrDefaultAsync();
            var idRoleUser = _context.UserRoles.Where(r => r.UserId == _userManager.GetUserId(HttpContext.User)).First().RoleId;
            var role = _context.Roles.Where(r => r.Id == idRoleUser).First().Name;
            var notification = user.Messages.Where(m => m.IsRead == false);
            return View(new UserViewModels() { Name = user.Name, Email = user.Email, LastName = user.LastName, Path = user.Path, Role = role, Notification = notification.Count() });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditUser([Bind("Name,LastName, Email, File")] UserViewModels userView )
        {

            var user = await _context.Users.Include(u => u.Messages).Where(u => u.Id == _userManager.GetUserId(HttpContext.User)).FirstOrDefaultAsync();
            var idRoleUser = _context.UserRoles.Where(r => r.UserId == _userManager.GetUserId(HttpContext.User)).First().RoleId;
            var role = _context.Roles.Where(r => r.Id == idRoleUser).First().Name;
            var notification = user.Messages.Where(m => m.IsRead == false);
            userView.Role = role;
            userView.Path = user.Path;
            userView.Notification = notification.Count();
            if (ModelState.IsValid)
            {
                try
                {

                    if (userView.File != null )
                    {

                        if (userView.File.Length > 0)
                        {
                            string filename = Guid.NewGuid() + Path.GetExtension(userView.File.FileName);
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\imgUser", filename);

                            using (var filestream = new FileStream( filePath, FileMode.Create))
                            {
                                await userView.File.CopyToAsync(filestream);
                            }

                            user.Path = "~/imgUser/" + filename;

                        }
                    }
                    user.Name = userView.Name;
                    user.Email = userView.Email;
                    user.LastName = userView.LastName;
                    user.UserName = userView.Email;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return View(userView);
                }
                return RedirectToAction(nameof(Index));
            }
            else 
            {
               return View(userView);

            }
           
        }

  

        [Authorize]
        public async Task<IActionResult> EditPassword()
        {
            var user = await _context.Users.Include(u => u.Messages).Where(u => u.Id == _userManager.GetUserId(HttpContext.User)).FirstOrDefaultAsync();
            var idRoleUser = _context.UserRoles.Where(r => r.UserId == user.Id).First().RoleId;
            var role = _context.Roles.Where(r => r.Id == idRoleUser).First().Name;
            var notification = user.Messages.Where(m => m.IsRead == false);

            return View(new ChangePasswordViewModel() { Role = role, Path = user.Path, Notification = notification.Count() });
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditPassword([Bind("ActualPassword,Password,ConfirmPassword")] ChangePasswordViewModel changePassword)
        {

            var user = await _context.Users.Include(u => u.Messages).Where(u => u.Id == _userManager.GetUserId(HttpContext.User)).FirstOrDefaultAsync();
            var idRoleUser = _context.UserRoles.Where(r => r.UserId == user.Id).First().RoleId;
            var role = _context.Roles.Where(r => r.Id == idRoleUser).First().Name;
            var notification = user.Messages.Where(m => m.IsRead == false);

            if (ModelState.IsValid)
            {
                IdentityResult result = await _userManager.ChangePasswordAsync(user, changePassword.ActualPassword, changePassword.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else 
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(String.Empty, error.Description);
                    }

                    return View(new ChangePasswordViewModel() { Role = role, Path = user.Path, Notification = notification.Count(), Password = changePassword.Password, ConfirmPassword = changePassword.ConfirmPassword });
                }
            }
            else
            {
                return View(new ChangePasswordViewModel() { Role = role, Path = user.Path, Notification = notification.Count(), Password = changePassword.Password, ConfirmPassword = changePassword.ConfirmPassword });
            }




        }



 
        /// 
        /// Admin view
        /// 


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListOfUser()
        {
            var users = await _context.Users.ToListAsync();

            var piloteRole = await _context.Roles.Where(r => r.Name == "Pilote").FirstAsync();
            var adminRole = await _context.Roles.Where(r => r.Name == "Admin").FirstAsync();

            IList<FakeUser> usersList = new List<FakeUser>();

            foreach (var user in users)
            {

                string work = _context.UserRoles.Where(r => r.UserId == user.Id).First().RoleId;
                if (piloteRole.Id == work)
                {
                    usersList.Add(new FakeUser { Email = user.Email, LastName = user.LastName, Name = user.Name, Role = "Pilot", Path = user.Path, IdUser = user.Id });
                }
                else if (adminRole.Id == work)
                {
                    usersList.Add(new FakeUser { Email = user.Email, LastName = user.LastName, Name = user.Name, Role = "Admin", Path = user.Path, IdUser = user.Id });
                }
                else
                {
                    usersList.Add(new FakeUser { Email = user.Email, LastName = user.LastName, Name = user.Name, Role = "Voyageur", Path = user.Path, IdUser = user.Id });
                }

            }


            return View(usersList);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUserDashboard(string id) 
        {
            if (id == null)
            {
                return RedirectToAction(nameof(ListOfUser));
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) 
            {
                return RedirectToAction(nameof(ListOfUser));
            }

            return View(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
     
        public async Task<IActionResult> EditUserDashboard([Bind("Id,Name,LastName,Email,Path")]User user, bool remove)
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (user == null)
            {
                return View(user);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (remove && user.Path != "~/img/user.png")
                    {
                        currentUser.Path = "~/img/user.png";
                    }
                    currentUser.Name = user.Name;
                    currentUser.Email = user.Email;
                    currentUser.LastName = user.LastName;
                    currentUser.UserName = user.Email;
                    _context.Update(currentUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return View(user);
                }
                return RedirectToAction(nameof(ListOfUser));
            }
            return View(user);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(ListOfUser));
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return RedirectToAction(nameof(ListOfUser));
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {

            if (id == null)
            {
                return RedirectToAction(nameof(ListOfUser));
            }


            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return RedirectToAction(nameof(ListOfUser));
            }


            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListOfUser));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Information(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(ListOfUser));
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return RedirectToAction(nameof(ListOfUser));
            }

            return View(user);
        }



    }
}
