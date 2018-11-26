using Malden.Portal.BLL;
using Malden.Portal.BLL.Utilities;
using Malden.Portal.GUI.Azure.Webrole.Models;
using Malden.Portal.GUI.Azure.Webrole.Utilities;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace Malden.Portal.GUI.Azure.Webrole.Controllers
{
    public class HomeController : AuthoriseController
    {
        private readonly IProductLogic _productLogic;
        private readonly IReleaseLogic _releaseLogic;
        private readonly IUserLogic _userLogic;
        private readonly IUserPurchaseLogic _userPurchaseLogic;
        private readonly IHistoryLogic _historyLogic;
        private readonly IProductCatalogueLogic _serialLogic;
        private readonly IBlobManagerLogic _blobLogic;

        public HomeController(IUserPurchaseLogic userPurchaseLogic, IReleaseLogic releaseLogic, IProductLogic productLogic, IUserLogic userLogic, IHistoryLogic historyLogic, 
            IProductCatalogueLogic serialLogic, IBlobManagerLogic blobLogic)
        {
            _userLogic = userLogic;
            _userPurchaseLogic = userPurchaseLogic;
            _releaseLogic = releaseLogic;
            _productLogic = productLogic;
            _historyLogic = historyLogic;
            _serialLogic = serialLogic;
            _blobLogic = blobLogic;
        }


        [NoCache]
        public ActionResult ArchiveDistributor(string id)
        {
            var releases = _releaseLogic.Archive(id);

            foreach (var release in releases)
            {
                release.ReleaseImageFiles = _blobLogic.CloudFiles(release.Id); //CloudFilesModel.FilteredFiles(release.Id, _releaseLogic, _productLogic, _blobLogic);
            }

            if (releases.Count > 0)
            {
                ViewBag.Productname = _productLogic.GetById(id).Name;
            }

            return View("Archive", releases);
        }

        //[OutputCache(CacheProfile = "Cache10Minutes")]
        [NoCache]
        public ActionResult Archive(int id)
        {
            var releases = _releaseLogic.OldReleasesByDate(id);

            foreach (var release in releases)
            {
                release.ReleaseImageFiles = _blobLogic.CloudFiles(release.Id); //CloudFilesModel.FilteredFiles(release.Id, _releaseLogic, _productLogic, _blobLogic);
            }

            TempData.Add("serial", id);
            if (releases.Count > 0)
            {
                ViewBag.Productname = _productLogic.GetById(releases[0].ProductId.ToString()).Name;
            }

            return View(releases);
        }

        public ActionResult Create()
        {
            return View("Index");
        }

        [HttpPost]
        public JsonResult IsValidSerial(string id)
        {
            if (!_userPurchaseLogic.IsValidSerialNumber(User.Identity.Name, id.Trim().ToUpper(), false))
                return Json("False", JsonRequestBehavior.AllowGet);
            else
                return Json("True", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Create(UserPurchase userPurchase)
        {
            ViewBag.ErrorMessage = "";
            var userEntity = _userLogic.User(User.Identity.Name);


            if (User.Identity.IsAuthenticated && userEntity != null)
            {
                ViewBag.userName = userEntity.Name;
            }

            var registrationCodeKey = "RegistrationCode";

            if (!ModelState.IsValid) return RedirectToAction("Index");

            if (userPurchase.RegistrationCode.Trim().Length <= 0)
            {
                ModelState.AddModelError(registrationCodeKey, "Serial number field is required.");
                ViewBag.ErrorMessage = "Serial number field is required.";
                return View("Create", userPurchase);
            }


            

            var isValidSerialNumber = _userPurchaseLogic.IsValidSerialNumber(User.Identity.Name, userPurchase.RegistrationCode.Trim().ToUpper());

            if (!isValidSerialNumber)
            {
                ViewBag.ErrorMessage = "Invalid serial number.";
                ModelState.AddModelError(registrationCodeKey, "Invalid serial number.");
                return View("Create", userPurchase);
            }

            try
            {
                var newPurchase = new UserPurchase(userPurchase.RegistrationCode.Trim().ToUpper(), User.Identity.Name);
                _userPurchaseLogic.Add(newPurchase, User.Identity.Name);

                return RedirectToAction("Index");
            }

            catch (DuplicateEntryException ex)
            {
                ModelState.AddModelError(registrationCodeKey, ex.Message);
                ViewBag.ErrorMessage = ex.Message;
            }
            catch (NotFoundException ex)
            {
                ModelState.AddModelError(registrationCodeKey, ex.Message);
                ViewBag.ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                ModelState.AddModelError(registrationCodeKey, ex.Message);
                ViewBag.ErrorMessage = ex.Message;
            }
            
            return View("Index", _userPurchaseLogic.List(User.Identity.Name));
        }

        public FileStreamResult DownloadRelease(int id)
        {
            var user = User.Identity.Name;
            try
            {
                var result = _releaseLogic.Download(User.Identity.Name, id);
                return File(result, "application/octet-stream", "malden-release");
            }
            catch
            {
                this.Flash("Errors occurred while processing your request!", FlashEnum.Error);
                Response.Redirect("~/");
                return null;
            }
        }

        [HttpPost]
        public JsonResult AddHistory(string id, string userType)
        {
            try
            {
                var indexOfHash = id.IndexOf("#");

                var customerUserType = ((int)Malden.Portal.BLL.User.UserType.Customer).ToString();
                var indexOfTilda = 0;
                var serial = 0;

                if (userType == customerUserType)
                {
                    indexOfTilda = id.IndexOf("~") + 1;
                    serial = Convert.ToInt32(id.Substring(0, indexOfTilda));
                }

                var releaseId = id.Substring(indexOfHash + 1, 36);
                var fileType = userType == customerUserType ? Convert.ToInt32(id.Substring(indexOfTilda, indexOfHash - (indexOfTilda + 1))) : Convert.ToInt32(id.Substring(indexOfTilda, indexOfHash - (indexOfTilda)));

                var release = _releaseLogic.GetById(releaseId);
                var serialInfo = _serialLogic.GetIdBySerialNumber(Convert.ToInt32(serial));

                _historyLogic.Add(new History { SerialNumber = Convert.ToInt32(serial), Version = release.Version, UserEmail = User.Identity.Name, TimeStamp = DateTime.UtcNow, ImageFileType = (Release.ImageFileType)fileType, ReleaseDownloaded = release });

                return Json("Ok", JsonRequestBehavior.AllowGet);
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

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult Filter()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult FileSize(string releaseId)
        {
            return Json("File size: 60.0mb", JsonRequestBehavior.AllowGet);
        }


        public ActionResult DistributorDownloads(string id)
        {
            var model = _historyLogic.List(id);

            return View("Admin", model);
        }

        //[OutputCache(CacheProfile="Cache10Minutes")]
        [NoCache]
        public ActionResult Index()
        {
            try
            {
                Session.Add("DemoDatabse", _userLogic.IsDemoDatabase() ? "DEMO DATABASE" : "");
                var userEntity = _userLogic.User(User.Identity.Name);

               
                 if (User.Identity.IsAuthenticated && userEntity != null)
                {
                    ViewBag.userName = userEntity.Name;

                    switch (userEntity.TypeOfUser)
                    {
                        case BLL.User.UserType.Admin:
                            Session.Add("TotalDownloads",_historyLogic.TotalDownloads());
                            return View("Admin", (_historyLogic.List(20)));

                        case BLL.User.UserType.Customer:
                            return View(_userPurchaseLogic.List(userEntity.Email));

                        case BLL.User.UserType.Distributor:
                            return View("Distributor", ProcessReleases());
                    }

                    return View(_userPurchaseLogic.List(userEntity.Email));
                }
                else
                {
                    FormsAuthentication.SignOut();
                    Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                    Response.AddHeader("pragma", "no-cache");
                    Response.AddHeader("Cache-Control", "no-cache");
                    Response.CacheControl = "no-cache";
                    Response.Expires = -1;
                    Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1);
                    Response.Cache.SetNoStore();
                    FormsAuthentication.RedirectToLoginPage();
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (NotFoundException ex)
            {
                this.Flash(ex.Message, FlashEnum.Error);
                return View("Create");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);

                this.Flash("Errors occurred while loading the registered products", FlashEnum.Error);
                return View("Create");
            }
        }

        private List<DistributorViewModel> ProcessReleases()
        {
            var releases = _userPurchaseLogic.LatestReleasesForAllProducts();
            var model = new List<DistributorViewModel>();
            foreach (var release in releases)
            {
                model.Add(new DistributorViewModel { Release = release, Product = _productLogic.GetById(release.ProductId) });
            }
            return model;
        }

        public JsonResult IsValidSerialNumber(string serial)
        {
            var isValidSerialNumber = _userPurchaseLogic.IsValidSerialNumber(User.Identity.Name, serial);

            return Json(isValidSerialNumber, JsonRequestBehavior.AllowGet);
        }


        public JsonResult HistoryRecords(string lastRowNumber)
        {
            var history = _historyLogic.List(Convert.ToInt32(lastRowNumber), 20);
            foreach (var h in history)
            {
                h.DateTimeStr = h.TimeStamp.ToString("dd-MMM-yyyy HH:mm:ss");
                h.VersionStr = h.Version != null ? h.Version.ToString() : "0.0";
                h.ImageFileTypeStr = Enum.GetName(typeof(Malden.Portal.BLL.Release.ImageFileType), h.ImageFileType);
                h.UserTypeStr = CustomHelpers.GetEnumDescription(h.UserType);
            }


            return Json(history, JsonRequestBehavior.AllowGet);
        }


        public FileStreamResult OldRelease(string id)
        {
            if (id == null)
            {
                this.Flash("Release details not found!", FlashEnum.Error);
                Response.Redirect("~/");
                return null;
            }

            try
            {
                var index = id.IndexOf("~") + 1;
                var serialNumber = id.Substring(index, id.Length - index);
                var prodId = id.Substring(0, index - 1);

                var result = _releaseLogic.Download(User.Identity.Name, prodId, Convert.ToInt32(serialNumber));
                return File(result, "application/octet-stream", "oldrelease");
            }
            catch
            {
                this.Flash("Internal server error", FlashEnum.Error);
                Response.Redirect("~/");
                return null;
            }
        }

        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(string emailAddress)
        {
            return View();
        }
    }
}