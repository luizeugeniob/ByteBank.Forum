using ByteBank.Forum.Models;
using ByteBank.Forum.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ByteBank.Forum.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        public UserManager<ApplicationUser> UserManager
        {
            get
            {
                if (_userManager == null)
                {
                    var contextOwin = HttpContext.GetOwinContext();
                    _userManager = contextOwin.GetUserManager<UserManager<ApplicationUser>>();
                }
                return _userManager;
            }
            set
            {
                _userManager = value;
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(AccountRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newUser = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    FullName = model.FullName
                };

                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user != null)
                    return View("WaitingConfirmation");

                var result = await _userManager.CreateAsync(newUser, model.Password);

                if (result.Succeeded)
                {
                    await SendConfirmationEmailAsync(user);

                    return View("WaitingConfirmation");
                }
                else
                {
                    AddErrors(result);
                }

            }

            return View(model);
        }

        public async Task<ActionResult> EmailConfirmation(string userId, string token)
        {
            if (userId == null || token == null)
                return View("Error");

            var result = await UserManager.ConfirmEmailAsync(userId, token);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            return View("Error");
        }

        private async Task SendConfirmationEmailAsync(ApplicationUser user)
        {
            var token = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

            var callbackUrl =
                Url.Action(
                   "EmailConfirmation",
                   "Account",
                   new { userId = user.Id, token = token },
                   Request.Url.Scheme);

            await UserManager.SendEmailAsync(
                user.Id,
                "Fórum ByteBank - Confirmação de Email",
                $"Bem vindo ao fórum ByteBank, clique aqui {callbackUrl} para confirmar seu email!");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}