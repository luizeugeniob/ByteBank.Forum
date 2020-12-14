using System.Web.Mvc;

namespace ByteBank.Forum.Controllers
{
    [Authorize(Roles = RolesNames.Admin)]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}