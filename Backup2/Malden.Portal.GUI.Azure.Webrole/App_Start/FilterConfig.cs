using System.Web.Mvc;

namespace Malden.Portal.GUI.Azure.Webrole
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}