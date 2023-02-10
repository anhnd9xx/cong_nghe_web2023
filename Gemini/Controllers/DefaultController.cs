using System.Web.Mvc;

namespace Gemini.Controllers
{
    public class DefaultController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return Json("Ping", JsonRequestBehavior.AllowGet);
        }
    }
}