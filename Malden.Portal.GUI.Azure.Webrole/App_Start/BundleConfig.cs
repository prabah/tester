using System.Web.Optimization;

namespace Malden.Portal.GUI.Azure.Webrole
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/Custom/jquery.maskedinput-1.3.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery/custom").Include(
                "~/Scripts/jquery-ui-1.10.3.js",
                        "~/Scripts/jquery.maskedinput-1.3.1.js",
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/Custom/app-insight.js",
                        "~/Scripts/jquery-ui-1.10.3.min.js",
                        "~/Scripts/Custom/jquery.rcrumbs.js"
                        ));

            //TODO: Change the view to use this bundle config
            //bundles.Add(new ScriptBundle("~/bundles/jquery/create").Include(
            //            "~/Scripts/jquery-1.8.2.min.js",
            //            "~/Scripts/jquery.validate.min.js",
            //            "~/Scripts/jquery.validate.unobtrusive.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //Custom bundles
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/Custom/validation-rules.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(

                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css"
                        //,"~/Content/themes/base/jquery.ui.theme.css"
                        ));

            //tooltip
            bundles.Add(new ScriptBundle("~/bundles/tooltips").Include("~/Scripts/Custom/tooltip.js"));

        }
    }
}