using Fir.App.ViewModels;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Fir.App.Controllers
{
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }
            AppUser appUser = new AppUser
            {
                Name = register.Name,
                Email = register.Email,
                Surname = register.Surname,
                UserName = register.UserName,
            };
            var result = await _userManager.CreateAsync(appUser,register.Password);
            if(!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    return View(register);
                }
            }
            await _userManager.AddToRoleAsync(appUser, "User");
            return RedirectToAction("index", "home");
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }
            AppUser appUser = await _userManager.FindByNameAsync(login.UserName);

            if(appUser is null)
            {
                ModelState.AddModelError("","Username or password is not correct ");
                return View(login);
            }
            var result = await _signInManager.PasswordSignInAsync(appUser, login.Password, login.RememberMe, true);
            if(!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Your account blocked 5 minutes");
                    return View(login);

                }
                ModelState.AddModelError("", "Username or password is not correct ");
                return View(login);
            }
            return RedirectToAction("index", "home");
        }
        //public async Task<IActionResult> CreateRole()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole
        //    {
        //        Name = "SuperAdmin"
        //    });
        //    await _roleManager.CreateAsync(new IdentityRole
        //    {
        //        Name = "Admin"
        //    });
        //    await _roleManager.CreateAsync(new IdentityRole
        //    {
        //        Name = "User"
        //    });

        //    return Json("Ok");
        //}

    }
}
