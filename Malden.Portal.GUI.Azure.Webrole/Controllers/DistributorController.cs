using Malden.Portal.BLL;
using Malden.Portal.GUI.Azure.Webrole.Utilities;
using System.Linq;
using System.Web.Mvc;


namespace Malden.Portal.GUI.Azure.Webrole.Controllers
{
    [CustomAttributes.AdminOnly]
    public class DistributorController : AuthoriseController
    {
        private readonly IDistributorLogic _distributorLogic;

        public DistributorController(IDistributorLogic distributorLogic)
        {
            _distributorLogic = distributorLogic;
        }
        
        public ActionResult Index()
        {
            return View(_distributorLogic.List().OrderByDescending(c => c.Email));
        }

        public ActionResult Details(string id)
        {
            try
            {
                return View(_distributorLogic.Get(id));
            }
            catch (NotFoundException e)
            {
                return Error(e.Message);
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Distributor model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var id = _distributorLogic.Add(model);
                return RedirectToAction("Details", "Distributor", new { id = id });
            }
            catch (DuplicateEntryException d)
            {
                ModelState.AddModelError("Email", d.Message);
                return View(model);
            }
        }

        public ActionResult Edit(string id)
        {
            var model = _distributorLogic.Get(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Distributor model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _distributorLogic.Update(model);
            return RedirectToAction("Details", "Distributor", new { id = model.Id }); ;
        }

        public ActionResult Error(string errorMessage)
        {
            ViewBag.Error = errorMessage;
            return View();
        }
    }
}
