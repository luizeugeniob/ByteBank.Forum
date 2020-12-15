using ByteBank.Forum.Models;
using ByteBank.Forum.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ByteBank.Forum.Controllers
{
    [Authorize(Roles = RolesNames.Admin)]
    public class UserController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        public UserManager<ApplicationUser> UserManager
        {
            get
            {
                if (_userManager == null)
                    _userManager = HttpContext.GetOwinContext().GetUserManager<UserManager<ApplicationUser>>();

                return _userManager;
            }

            set { _userManager = value; }
        }

        private RoleManager<IdentityRole> _roleManager;
        public RoleManager<IdentityRole> RoleManager
        {
            get
            {
                if (_roleManager == null)
                    _roleManager = HttpContext.GetOwinContext().GetUserManager<RoleManager<IdentityRole>>();

                return _roleManager;
            }

            set { _roleManager = value; }
        }

        public ActionResult Index()
        {
            var users =
                UserManager
                    .Users
                    .ToList()
                    .Select(user => new UserViewModel(user));

            return View(users);
        }

        public async Task<ActionResult> EditRoles(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            var model = new UserEditRolesViewModel(user, RoleManager);

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditRoles(UserEditRolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(model.Id);

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                var removeResult = 
                    await UserManager.RemoveFromRolesAsync(
                        user.Id,
                        userRoles.ToArray());

                if (removeResult.Succeeded)
                {
                    var rolesSelectedByAdmin = model
                        .UserRoles
                        .Where(role => role.Selected)
                        .Select(role => role.Name)
                        .ToArray();

                    var addResult = await UserManager.AddToRolesAsync(
                        user.Id,
                        rolesSelectedByAdmin);

                    if (addResult.Succeeded)
                        return RedirectToAction("Index");
                }
            }

            return View();
        }
    }
}