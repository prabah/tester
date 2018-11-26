using Malden.Portal.BLL;
using Malden.Portal.BLL.Utilities;
using Malden.Portal.GUI.Azure.Webrole.Utilities;
using System;
using System.Web.Mvc;

namespace Malden.Portal.GUI.Azure.Webrole.Controllers
{
    [CustomAttributes.AdminOnly]
    public class ProductController : AuthoriseController
    {
        private readonly IProductLogic _productLogic;

        public ProductController()
        {
        }

        public ProductController(IProductLogic productLogic)
        {
            _productLogic = productLogic;
        }

        public ActionResult Index()
        {
            return View(_productLogic.List());
        }

        public ActionResult Edit(string id)
        {
            return View(_productLogic.GetById(id));
        }

        [HttpPost]
        public ActionResult Edit(Product model)
        {
            try
            {
                if (!ModelState.IsValid) return View();

                TryUpdateModel<Product>(model);

                _productLogic.Update(model);

                return RedirectToAction("Index");
            }

            catch (Exception exception)
            {
                ErrorLogger.Log(exception);
                this.Flash("Errors occurred during the process!", FlashEnum.Error);

                return View();
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Product model)
        {
            try
            {
                if (!ModelState.IsValid) return View();

                TryUpdateModel<Product>(model);

                _productLogic.Add(model);

                return RedirectToAction("Index");
            }
            catch (DuplicateEntryException exception)
            {
                this.Flash(exception.Message, FlashEnum.Error);
                return View();
            }

            catch (Exception exception)
            {
                ErrorLogger.Log(exception);
                this.Flash("Errors occurred during the process!", FlashEnum.Error);

                return View();
            }
        }

        public ActionResult Details(string id)
        {
            return View(_productLogic.GetById(id));
        }

        [HttpPost]
        public JsonResult Delete(string id)
        {
            try
            {
                _productLogic.Delete(id);

                return Json("Deleted", JsonRequestBehavior.AllowGet);
            }
            catch (ReferenceException exception)
            {
                this.Flash(exception.Message, FlashEnum.Error);
                return Json(exception.Message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                var exType = exception.GetType();
                ErrorLogger.Log(exception);
                this.Flash("Errors occurred during the process!", FlashEnum.Error);
                return Json(exception.Message, JsonRequestBehavior.AllowGet);
            }
        }
    }
}