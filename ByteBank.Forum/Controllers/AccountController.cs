using ByteBank.Forum.Models;
using ByteBank.Forum.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
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

        private SignInManager<ApplicationUser, string> _signInManager;
        public SignInManager<ApplicationUser, string> SignInManager
        {
            get
            {
                if (_signInManager == null)
                {
                    var contextOwin = HttpContext.GetOwinContext();
                    _signInManager = contextOwin.GetUserManager<SignInManager<ApplicationUser, string>>();
                }
                return _signInManager;
            }
            set
            {
                _signInManager = value;
            }
        }

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return Request.GetOwinContext().Authentication;
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
                    await SendConfirmationEmailAsync(newUser);

                    return View("WaitingConfirmation");
                }
                else
                {
                    AddErrors(result);
                }

            }

            return View(model);
        }

        [HttpPost]
        public ActionResult RegisterByExternalAuthentication(string provider)
        {
            SignInManager.AuthenticationManager.Challenge(new AuthenticationProperties
            {
                RedirectUri = Url.Action("RegisterByExternalAuthenticationCallback")
            }, provider);

            return new HttpUnauthorizedResult();
        }

        public async Task<ActionResult> RegisterByExternalAuthenticationCallback()
        {
            var loginInfo = await SignInManager.AuthenticationManager.GetExternalLoginInfoAsync();

            var exsistingUser = await UserManager.FindByEmailAsync(loginInfo.Email);
            if (exsistingUser != null)
            {
                return View("Error");
            }

            var user = new ApplicationUser
            {
                Email = loginInfo.Email,
                UserName = loginInfo.Email,
                FullName = loginInfo.ExternalIdentity.FindFirstValue(
                    loginInfo.ExternalIdentity.NameClaimType)
            };

            var result = await UserManager.CreateAsync(user);
            if (result.Succeeded)
            {
                var resultAddLogin =
                    await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                if (resultAddLogin.Succeeded)
                    return RedirectToAction("Index", "Home");
            }

            return View("Error");
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

        public async Task<ActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(AccountLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);

                if (user == null)
                    return InvalidEmailOrPassword();

                var result =
                    await SignInManager.PasswordSignInAsync(
                        user.UserName,
                        model.Password,
                        isPersistent: model.KeepLogged,
                        shouldLockout: true);

                switch (result)
                {
                    case SignInStatus.Success:

                        if (!user.EmailConfirmed)
                        {
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            return View("WaitingConfirmation");
                        }

                        return RedirectToAction("Index", "Home");

                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("TwoFactorVerification");

                    case SignInStatus.LockedOut:
                        return await UserLockedOutAsync(user, model);

                    default:
                        return InvalidEmailOrPassword();
                }
            }

            return View(model);
        }

        public async Task<ActionResult> TwoFactorVerification()
        {
            var result = await SignInManager.SendTwoFactorCodeAsync("SMS");

            if (result)
                return View();

            return View("Error");
        }

        [HttpPost]
        public async Task<ActionResult> TwoFactorVerification(string token)
        {
            var result = 
                await SignInManager.TwoFactorSignInAsync(
                    "SMS",
                    token,
                    isPersistent: false,
                    rememberBrowser: false);

            if (result == SignInStatus.Success)
                return RedirectToAction("Index", "Home");

            return View("Error");
        }

        [HttpPost]
        public ActionResult LoginByExternalAuthentication(string provider)
        {
            SignInManager.AuthenticationManager.Challenge(new AuthenticationProperties
            {
                RedirectUri = Url.Action("LoginByExternalAuthenticationCallback")
            }, provider);

            return new HttpUnauthorizedResult();
        }

        public async Task<ActionResult> LoginByExternalAuthenticationCallback()
        {
            var loginInfo = await SignInManager.AuthenticationManager.GetExternalLoginInfoAsync();

            var result = await SignInManager.ExternalSignInAsync(loginInfo, true);

            if (result == SignInStatus.Success)
                return RedirectToAction("Index", "Home");

            return View("Error");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(AccountForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

                    var callbackUrl =
                        Url.Action(
                           "ResetPassword",
                           "Account",
                           new { userId = user.Id, token = token },
                           Request.Url.Scheme);

                    await UserManager.SendEmailAsync(
                        user.Id,
                        "Fórum ByteBank - Alteração de Senha",
                        $"Clique aqui {callbackUrl} para alterar sua senha!");
                }

                return View("ResetPasswordEmailSent");
            }

            return View();
        }

        public ActionResult ResetPassword(string userId, string token)
        {
            var model = new AccountResetPasswordViewModel
            {
                UserId = userId,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(AccountResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await UserManager.ResetPasswordAsync(
                        model.UserId,
                        model.Token,
                        model.NewPassword);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                AddErrors(result);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Logoff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        private ActionResult InvalidEmailOrPassword()
        {
            ModelState.AddModelError("", "Email ou senha incorretos");
            return View("Login");
        }

        private async Task<ActionResult> UserLockedOutAsync(ApplicationUser user, AccountLoginViewModel model)
        {
            var isPasswordCorrect =
                await UserManager.CheckPasswordAsync(
                    user,
                    model.Password);

            if (isPasswordCorrect)
            {
                ModelState.AddModelError("", "A conta está bloqueada!");
                return View("Login");
            }
            else
                return InvalidEmailOrPassword();
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

        public async Task<ActionResult> MyAccount()
        {
            var userId = HttpContext.User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);

            var model = new AccountMyAccountViewModel
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                TwoFactorEnabled = user.TwoFactorEnabled,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> MyAccount(AccountMyAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.User.Identity.GetUserId();
                var user = await UserManager.FindByIdAsync(userId);

                user.FullName = model.FullName;
                user.PhoneNumber = model.PhoneNumber;

                if (!user.PhoneNumberConfirmed)
                    await SendConfirmationSmsAsync(user);
                else
                    user.TwoFactorEnabled = model.TwoFactorEnabled;

                var result = await UserManager.UpdateAsync(user);

                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");

                AddErrors(result);
            }

            return View();
        }

        private async Task SendConfirmationSmsAsync(ApplicationUser user)
        {
            var confirmationToken =
                await UserManager.GenerateChangePhoneNumberTokenAsync(
                    user.Id,
                    user.PhoneNumber
                );

            await UserManager.SendSmsAsync(
                user.Id,
                $"Token de confirmação: {confirmationToken}");
        }

        public ActionResult VerificationCode()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> VerificationCode(string token)
        {
            var userId = HttpContext.User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);

            var result = await UserManager.ChangePhoneNumberAsync(userId, user.PhoneNumber, token);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            AddErrors(result);

            return View();
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