using ByteBank.Forum.Models;
using ByteBank.Forum.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ByteBank.Forum.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<ActionResult> Register(AccountRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dbContext = new IdentityDbContext<ApplicationUser>("DefaultConnection");
                var userStore = new UserStore<ApplicationUser>(dbContext);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var newUser = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    FullName = model.FullName
                };

                await userManager.CreateAsync(newUser, model.Password);

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}