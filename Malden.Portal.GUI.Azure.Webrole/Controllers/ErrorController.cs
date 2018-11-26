using System.Web.Mvc;

namespace Malden.Portal.GUI.Azure.Webrole.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult HttpError()
        {
            return View("Error");
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult CustomError()
        {
            return View();
        }

    }
}
