using ByteBank.Forum.ViewModels;
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
        public ActionResult Register(AccountRegisterViewModel accountRegisterViewModel)
        {
            return View();
        }
    }
}