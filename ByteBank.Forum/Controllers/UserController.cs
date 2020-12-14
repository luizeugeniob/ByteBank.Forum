using ByteBank.Forum.Models;
using ByteBank.Forum.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
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

        public ActionResult Index()
        {
            var users =
                UserManager
                    .Users
                    .ToList()
                    .Select(user => new UserViewModel(user));

            return View(users);
        }

        public ActionResult EditRoles(string id)
        {
            return View();
        }
    }
}