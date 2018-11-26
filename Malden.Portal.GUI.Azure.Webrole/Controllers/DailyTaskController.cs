using Malden.Portal.BLL;
using System.Web.Mvc;

namespace Malden.Portal.GUI.Azure.Webrole.Controllers
{
    public class DailyTaskController : Controller
    {
        IUserLogic _userLogic;

        public DailyTaskController(IUserLogic userLogic)
        {
            _userLogic = userLogic;
        }

        public ActionResult CleanUpUsers()
        {
            _userLogic.DeleteInactiveUsers((int) Malden.Portal.BLL.User.UserType.Customer);

            return View();
        }

    }
}
