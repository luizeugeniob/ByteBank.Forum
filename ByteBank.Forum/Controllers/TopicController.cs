using ByteBank.Forum.ViewModels;
using System.Web.Mvc;

namespace ByteBank.Forum.Controllers
{
    public class TopicController : Controller
    {
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Create(TopicCreateViewModel model)
        {
            return View();
        }
    }
}