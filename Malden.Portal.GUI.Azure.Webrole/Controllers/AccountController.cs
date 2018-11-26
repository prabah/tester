using Malden.Portal.BLL;
using Malden.Portal.BLL.Utilities;
using Malden.Portal.GUI.Azure.Webrole.Mailers;
using Malden.Portal.GUI.Azure.Webrole.Models.Users;
using Malden.Portal.GUI.Azure.Webrole.Utilities;
using Microsoft.ApplicationInsights.Telemetry.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Malden.Portal.GUI.Azure.Webrole.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmailerLogic _emailLogic;
        private readonly IUserLogic _userLogic;
        private readonly IUserMailer _userMailer;
        private readonly IDistributorLogic _distributorLogic;

        public AccountController(IUserLogic userLogic, IEmailerLogic emailLogic, IUserMailer userMailer, IDistributorLogic distributorLogic)
        {
            _userLogic = userLogic;
            _emailLogic = emailLogic;
            _userMailer = userMailer;
            _distributorLogic = distributorLogic;
        }

        [Malden.Portal.GUI.Azure.Webrole.Utilities.CustomAttributes.AdminOnly]
        public ActionResult Administrators()
        {
            return View("Index", _userLogic.List(Malden.Portal.BLL.User.UserType.Admin));
        }


        [Malden.Portal.GUI.Azure.Webrole.Utilities.CustomAttributes.AdminOnly]
        public ActionResult Distributors()
        {
            return View("Index", _userLogic.List(Malden.Portal.BLL.User.UserType.Distributor));
        }

        [Malden.Portal.GUI.Azure.Webrole.Utilities.CustomAttributes.AdminOnly]
        public ActionResult Edit(string id)
        {
            var user = _userLogic.Get(id);

            Malden.Portal.BLL.User.UserType opts = new Malden.Portal.BLL.User.UserType();
            ViewBag.Selections = opts.ToSelectList();

            return View(user);
        }

        [HttpPost]
        [Malden.Portal.GUI.Azure.Webrole.Utilities.CustomAttributes.AdminOnly]
        public ActionResult Edit(User model, string UserType)
        {
            Malden.Portal.BLL.User.UserType opts = new Malden.Portal.BLL.User.UserType();
            ViewBag.Selections = opts.ToSelectList();

            try
            {
                ModelState.Remove("Password");

                if (!ModelState.IsValid) return View();
                TryUpdateModel<User>(model);
                model.TypeOfUser = (BLL.User.UserType)(Convert.ToInt32(UserType));

                _userLogic.Update(model);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                this.Flash("Errors occurred during the process!", FlashEnum.Error);

                return View();
            }
        }

        [Malden.Portal.GUI.Azure.Webrole.Utilities.CustomAttributes.AdminOnly]
        public ActionResult Index()
        {
            return View(_userLogic.List(Malden.Portal.BLL.User.UserType.Customer));
        }

        #region NewUser
        [AllowAnonymous]
        public ActionResult Activate(string id)
        {
            try
            {
                if (id == null) return View("ActivationError");

                if (_distributorLogic.IsDistributorAcccount(id))
                {
                    Session.Add("UserKey", id);
                    return RedirectToAction("ConfirmDistributorToken", new { id = id });
                }
                else
                {
                    _userLogic.ActivateUser(id, (int)Malden.Portal.BLL.EmailerLogic.EmailType.Welcome);
                    return View("ActivationSuccess");
                }
            }
            catch
            {
                return View("ActivationError");
            }

        }

        public ActionResult ActivateDistributor(string token)
        {
            if (token == null)
            {
                ModelState.AddModelError("token", "Please enter the token");
                return View();
            }

            var id = Session["UserKey"].ToString();

            _distributorLogic.ActivateDistributor(id, (int)Malden.Portal.BLL.EmailerLogic.EmailType.Welcome, _distributorLogic.EmailByDistributorToken(token), token);

            return View("ActivationSuccess");
        }

        [AllowAnonymous]
        public ActionResult ConfirmDistributorToken(string id)
        {
            try
            {
                ViewBag.UserKey = id;
                return View();
            }
            catch (InvalidTokenException ex)
            {
                ModelState.AddModelError("token", ex.Message);
                return View();
            }
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.RegisterForm = "True";
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(AccountModel model)
        {
            ViewBag.RegisterForm = "True";
            if (ModelState.IsValid)
            {

                if (_userLogic.UserAlreadyExixts(model.Email.ToLower()))
                {
                    ModelState.AddModelError("", "Email is already registered!");
                    return View(model);
                }

                var user = new User(model.Email.ToLower(), model.Password)
                {
                    Name = model.Name,
                    Company = model.Company,
                    IsBlocked = true,
                    TypeOfUser = Malden.Portal.BLL.User.UserType.Customer,
                    Id = Guid.NewGuid().ToString()
                };
                var uniqueId = Guid.NewGuid().ToString("N");

                _userLogic.Add(user, user.Id, Malden.Portal.BLL.User.UserType.Customer);

                ServerAnalytics.BeginRequest();
                var properties = new Dictionary<string, object>() { { "Send Start", model.Email }, { "Date & Time", DateTime.UtcNow.ToString() } };
                ServerAnalytics.CurrentRequest.LogEvent("MaldenPortal/Registrations", properties);

                _userMailer.Welcome(model.Name, UriString(uniqueId, EmailerLogic.EmailType.Welcome), user.Email).Send();

                properties = new Dictionary<string, object>() { { "Sent", model.Email }, { "Date & Time", DateTime.UtcNow.ToString() } };
                ServerAnalytics.CurrentRequest.LogEvent("MaldenPortal/Registrations", properties);

                _emailLogic.Add(user, uniqueId, EmailerLogic.EmailType.Welcome);

                return View("ActivationRequest");

            }
            return View(model);
        }

        #endregion

        #region PasswordForgotten
        [AllowAnonymous]
        public ActionResult PasswordResetRequest()
        {
            return View();
        }



        [HttpPost]
        [AllowAnonymous]
        public ActionResult PasswordResetRequest(EmailPasswordResetModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = _userLogic.GetByEmail(model.Email);

            if (user == null) return View("PasswordResetEmailed");

            var uniqueId = Guid.NewGuid().ToString("N");
            _emailLogic.Add(user, uniqueId, EmailerLogic.EmailType.ResetPassword);
            _userMailer.PasswordReset(model.Email, UriString(uniqueId, EmailerLogic.EmailType.ResetPassword)).Send();

            return View("PasswordResetEmailed");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ConfirmPasswordReset(PasswordResetModel model, string activationKey)
        {
            if (activationKey == null) return View();

            ViewData.Add("activationKey", activationKey);
            if (_userLogic.IsEmailInstanceActivated(activationKey, EmailerLogic.EmailType.ResetPassword))
            {
                ModelState.AddModelError("Changed", "Password is already changed for this request!");
                return View("ConfirmPasswordReset");
            }

            if (_userLogic.IsEmailInstanceExpired(activationKey, EmailerLogic.EmailType.ResetPassword))
            {
                ModelState.AddModelError("Changed", "Request has been expired!");
                return View("ConfirmPasswordReset");
            }

            if (ModelState.IsValid)
            {
                _userLogic.PasswordReset(activationKey, model.Password);
                return View("PasswordResetSuccess");
            }
            else
            {
                ModelState.AddModelError("Changed", "Errors occurred!");

                return View("ConfirmPasswordReset");
            }

        }

        [AllowAnonymous]
        public ActionResult ConfirmPasswordReset(string id)
        {
            ViewData.Add("activationKey", id);
            ViewBag.ActivationKey = id;
            return View();
        }

        [HttpPost]
        public ActionResult PasswordResetEmailed()
        {
            return View();
        }
        #endregion

        #region LoginAndLogout
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (!Request.IsAuthenticated)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(CustomLoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return View();

            var userLoginStatus = _userLogic.LoginStatus(model.Email.ToLower(), model.Password);

            switch (userLoginStatus)
            {
                case UserLogic.UserLoginStatus.Invalid:
                default:

                    var distributor = _distributorLogic.GetByEmail(model.Email);

                    if (distributor == null)
                    {
                        ModelState.AddModelError("", "The user name or password provided is incorrect.");
                        return View(model);
                    }
                    else
                    {
                        ViewBag.RegisterAsADistributor = "Please follow the instructions from Malden and register an account";
                        return RedirectToAction("Register");
                    }

                case UserLogic.UserLoginStatus.NotActivated:
                    ModelState.AddModelError("", "You must activate your account before you can use.");
                    return View(model);
                case UserLogic.UserLoginStatus.Blocked:
                    ModelState.AddModelError("", "Your account is locked, Please contact Malden Electronics to re-activate.");
                    return View(model);
                case UserLogic.UserLoginStatus.Valid:
                    Session.Add("UserName", _userLogic.User(model.Email.ToLower()).Name ?? "");

                    FormsAuthentication.SetAuthCookie(model.Email.ToLower(), true);
                    return RedirectToLocal(returnUrl);
            }

        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.AddHeader("pragma", "no-cache");
            Response.AddHeader("Cache-Control", "no-cache");
            Response.CacheControl = "no-cache";
            Response.Expires = -1;
            Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1);
            Response.Cache.SetNoStore();
            Response.Cookies.Remove(Response.Cookies[0].Name);

            string url = Request.UrlReferrer.AbsolutePath;

            FormsAuthentication.SignOut();
            return Redirect(url);

        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        private string UriString(string uniqueId, EmailerLogic.EmailType emailType)
        {
            var url = HttpContext.Request.Url.AbsoluteUri;
            var path = emailType == EmailerLogic.EmailType.Welcome ? "Activate" : "ConfirmPasswordReset";

            return String.Format("{0}{1}/{2}", url.Remove(url.Length - url.Split('/').Last().Length), path, uniqueId);
        }

        [HttpPost]
        public JsonResult Block(string id)
        {
            try
            {
                _userLogic.BlockUser(_userLogic.Get(id).Email);

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