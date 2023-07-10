using Fir.App.ViewModels;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Net.Mail;

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
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");

        }
        [Authorize]
        public async Task<IActionResult> Info()
        {
            string UserName = User.Identity.Name;
            AppUser appUser = await _userManager.FindByNameAsync(UserName);
            return View(appUser);

        }
        [Authorize]
        public async Task<IActionResult> Update()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if(user is null)
            {
                return NotFound();
            }
            UpdatedUserVM model = new UpdatedUserVM()
            {
                Name = user.Name,
                Email = user.Email,
                UserName = user.UserName,
                Surname = user.Surname
            };
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(UpdatedUserVM model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (user is null)
            {
                return NotFound();
            }
            user.Name = model.Name;
            user.Email = model.Email;
            user.Surname = model.Surname;
            user.UserName = model.UserName;

    
           var result =  await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.NewPassword)){
                 result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                    return View(model);
                }

            }
            await _signInManager.SignInAsync(user,true);

            return RedirectToAction(nameof(Info));
        }
        [HttpGet]
        public async Task<IActionResult> ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(string mail)
        {
            var user = await _userManager.FindByEmailAsync(mail);
            if (user is null)
            {
                return NotFound();
            }
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

           string? link= Url.Action(action: "ResetPassword", controller: "Account", values: new
            {
                token = token,
                mail = mail
            }, protocol: Request.Scheme);
    
            MailMessage mm = new MailMessage();
            mm.To.Add(user.Email);
            mm.Subject = "Reset Password";
            mm.Body = $"<a href='{link}'>Click me for reseting your password</a>";
            mm.IsBodyHtml = true;
            mm.From = new MailAddress("nicatsoltanli03@gmail.com");

            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential("nicatsoltanli03@gmail.com", "gmaagjxgczxovsrw");

            await smtp.SendMailAsync(mm);
            return RedirectToAction("index","home");
        }
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token,string mail)
        {
            var user = await _userManager.FindByEmailAsync(mail);
            if (user is null)
            {
                return NotFound();
            }
            ResetPasswordVM model = new ResetPasswordVM()
            {
                Token = token,
                Email = mail,

            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                return NotFound();
            }
             var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(model);
            }
            return RedirectToAction(nameof(Info));
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
