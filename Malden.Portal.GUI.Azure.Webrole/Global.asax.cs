using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Malden.Portal.GUI.Azure.Webrole
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }


        void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();

            if (ex is HttpRequestValidationException)
            {
                Response.Clear();
                Response.StatusCode = 200;
                Response.Write(@"[html]");
                Response.End();
            }
        }

        protected void Application_BeginRequest()
        {
#if (!DEBUG)
                {
                    if (FormsAuthentication.RequireSSL && !Request.IsSecureConnection)
                    {
                        Response.Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));
                    }
                }
#else
            {

            }
#endif
        }
        

    }
}