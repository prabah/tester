using Malden.Portal.BLL;
using Malden.Portal.GUI.Azure.Webrole.Models;
using Malden.Portal.GUI.Azure.Webrole.Utilities;
using System;
using System.Web.Mvc;

namespace Malden.Portal.GUI.Azure.Webrole.Controllers
{
    public class MaintenanceController : AuthoriseController
    {
        private IMaintenanceContractLogic _maintenanceLogic;
        private IProductCatalogueLogic _serialLogic;
        private IProductLogic _productLogic;

        public MaintenanceController(IMaintenanceContractLogic maintenanceLogic, IProductCatalogueLogic serialNumberLogic, IProductLogic productLogic)
        {
            _maintenanceLogic = maintenanceLogic;
            _serialLogic = serialNumberLogic;
            _productLogic = productLogic;
        }

        public ActionResult Index()
        {
            return View(_maintenanceLogic.List());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(int SerialNumber, DateTime DateOfExpiry)
        {
            if (SerialNumber <= 0 || DateOfExpiry <= DateTime.UtcNow)
            {
                this.Flash("Invalid maintenance contract details", FlashEnum.Warning);
                return View();
            }

            _maintenanceLogic.Add(new MaintenanceContract
            {
                DateOfExpiry = DateOfExpiry,
                SerialNumber = SerialNumber,
                SerialKeyId = _serialLogic.GetByKey(SerialNumber).Id,
                Id = Guid.NewGuid().ToString()
            });

            return View("Index", _maintenanceLogic.List());
        }

        public JsonResult Delete(string id)
        {
            _maintenanceLogic.Delete(id);
            return Json("Deleted", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Manage(int id)
        {
            var list = _maintenanceLogic.List(id);
            if (list.Count > 0)
            {
                return View(list);
            }
            else
            {
                this.Flash("No maintenance contract for the selected product is found", FlashEnum.Error);
                return View("Index", _maintenanceLogic.List());
            }
        }

        public ActionResult Edit(string id)
        {
            return View(_maintenanceLogic.Get(id));
        }

        [HttpPost]
        public ActionResult Edit(int SerialNumber, DateTime DateOfExpiry, MaintenanceContract model)
        {
            if (SerialNumber <= 0)
            {
                this.Flash("Invalid maintenance contract details", FlashEnum.Warning);
                return View();
            }

            _maintenanceLogic.UpdateById(new MaintenanceContract
            {
                DateOfExpiry = DateOfExpiry,
                SerialNumber = SerialNumber,
                SerialKeyId = _serialLogic.GetByKey(SerialNumber).Id,
                Id = model.Id
            });

            return View("Index", _maintenanceLogic.List());
        }

        public JsonResult SerialDetails(int id)
        {
            var serialDetails = _serialLogic.GetByKey(id);

            if (serialDetails != null)
            {
                var viewModel = new MaintenanceViewModel { SerialNumberDetails = serialDetails, Product = _productLogic.GetById(serialDetails.ProductId) };
                return Json(viewModel, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }
    }
}