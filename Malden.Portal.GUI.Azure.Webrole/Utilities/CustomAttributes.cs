using Malden.Portal.BLL;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Malden.Portal.GUI.Azure.Webrole.Utilities
{
    internal class CustomAttributes
    {
        public class CustomAuthoriseAttribute : AuthorizeAttribute
        {
            public override void OnAuthorization(AuthorizationContext filterContext)
            {
                if (filterContext == null)
                {
                    throw new ArgumentNullException("filterContext");
                }

                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    const string loginUrl = "/"; // Default Login Url
                    filterContext.Result = new RedirectResult(loginUrl);
                }
            }
        }

        public class ValidateOnlyIncomingValuesAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                var modelState = filterContext.Controller.ViewData.ModelState;

                var keysWithNoIncomingValue = modelState.Keys;
                foreach (var key in keysWithNoIncomingValue)
                    modelState[key].Errors.Clear();
            }
        }

        public class AdminOnlyAttribute : ActionFilterAttribute
        {
            [Authorize]
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                var userLogic = (IUserLogic)System.Web.Mvc.DependencyResolver.Current.GetService(typeof(IUserLogic));

                if (!userLogic.IsValidAdminUser(HttpContext.Current.User.Identity.Name))
                {
                    const string message = "Errors occurred!!";
                    var redirectTargetDictionary = new RouteValueDictionary
                                                       {
                                                           {"area", ""},
                                                           {"action", "Error"},
                                                           {"controller", "Home"},
                                                           {"customMessage", message}
                                                       };
                    filterContext.Result = new RedirectToRouteResult(redirectTargetDictionary);
                }
                base.OnActionExecuting(filterContext);
            }
        }

        public class RequireHttpsAttribute : System.Web.Mvc.RequireHttpsAttribute
        {
            public bool RunnigFromServer = false;
            

            public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
            {
                //if (DebugSecure)
                //{
                //    base.OnAuthorization(filterContext);
                //}
                //else
                //{
                //    // non secure requested
                //    if (filterContext.HttpContext.Request.IsSecureConnection)
                //    {
                //        HandleNonHttpRequest(filterContext);
                //    }
                //}

                if (RunnigFromServer)
                {
                    base.OnAuthorization(filterContext);
                }
                else
                {
                    if (filterContext.HttpContext.Request.IsSecureConnection)
                    {
                        HandleNonHttpRequest(filterContext);
                    }
                }

            }

            protected virtual void HandleNonHttpRequest(AuthorizationContext filterContext)
            {
                if (String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    // redirect to HTTP version of page
                    string url = "http://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
                    filterContext.Result = new RedirectResult(url);
                }

            }
        }
    }
}