using Malden.Portal.BLL;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Malden.Portal.GUI.Azure.Webrole.Utilities
{
    public enum FlashEnum
    {
        Success = 1,
        Info = 2,
        Warning = 3,
        Error = 4
    }

    public static class CustomHelpers
    {
        private static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static string SizeSuffix(long value)
        {
            if (value == 0) return "";

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1 << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static void Flash(this Controller controller, string message, FlashEnum type = FlashEnum.Success)
        {
            controller.TempData[string.Format("flash-{0}", type.ToString().ToLower())] = message;
        }

        public static string FindSerialNumberString(string target)
        {
            var str = new StringBuilder();

            str.Append("Find your product serial number in the Malden Key Manager application available from ");
            str.Append("<br />");
            str.Append("Start-Menu > Malden > Malden Tools > Key Manager or");
            str.Append("<br />");
            str.Append("Start-Menu > Malden Electronics > Malden Tools > Key Manager");

            return str.ToString();
        }

        public static MvcHtmlString AdminOnlyMenu(this HtmlHelper htmlHelper, string controllerName, string linkText, string actionName)
        {
            return MvcHtmlString.Create(System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, linkText, actionName, controllerName, null, new RouteValueDictionary()).ToHtmlString());
        }

        public static string MD5SerialNumber(int serialNumber)
        {
            return serialNumber.ToString("D5") + "-" + Malden.Portal.BLL.Utilities.PasswordResolver.CalculateMD5Hash(serialNumber.ToString()).Substring(0, 5);
        }

        public static string FormattedVersionString(string version)
        {
            var spaceText = "";
            if (version.Length < 6)
            {
                for (int i = 6 - version.Length; i <= 6; i++)
                {
                    spaceText += " ";
                }
            }

            return spaceText + version;
        }

        public static string ImageForDownlaodable(bool avaialbale)
        {
            var str = new StringBuilder();
            str.Append("<img ");
            str.Append(avaialbale ? "src = '/Content/Custom/images/active.png'>" : "src = '/Content/Custom/images/inactive.png'>");

            return str.ToString();
        }

        public static string DownloadLink(CloudFile cloudFile, string serialNumber, string availableReleaseId, string availableReleaseVersion, Malden.Portal.BLL.User.UserType userType = User.UserType.Customer)
        {
            var link = "";
            var buttonClass = "";
            var dataId = "";
            var targetStr = "";
            var title = "";
            var desc = "";
            var arrowClass = "";
            var userTypeString = ((int)userType).ToString();

            if (cloudFile != null)
            {
                buttonClass = cloudFile.ImageFileType == Release.ImageFileType.Notes ? "releasenotes" : "downloadRelease";

                if (userType == User.UserType.Customer)
                {
                    dataId = serialNumber + "~" + ((int)cloudFile.ImageFileType).ToString() + "#" + availableReleaseId;
                }
                else
                {
                    dataId = ((int)cloudFile.ImageFileType).ToString() + "#" + availableReleaseId;
                }

                targetStr = cloudFile.ImageFileType == Malden.Portal.BLL.Release.ImageFileType.Notes ? " target=_blank" : "";
                title = Malden.Portal.GUI.Azure.Webrole.Utilities.CustomHelpers.SizeSuffix(cloudFile.Size);
                var formattedVersionString = Malden.Portal.GUI.Azure.Webrole.Utilities.CustomHelpers.GetEnumDescription(@cloudFile.ImageFileType) + " " +
                                    "(" + availableReleaseVersion + ")";
                desc = @cloudFile.ImageFileType != Malden.Portal.BLL.Release.ImageFileType.Notes ?
                                    formattedVersionString : "Release Notes";
                arrowClass = @cloudFile.ImageFileType != Malden.Portal.BLL.Release.ImageFileType.Notes ?
                                    "arrow" : "arrow-notes";

                link = "<a href='" + cloudFile.URL + "' class=" + buttonClass + " data-id= " + dataId + targetStr + " data-user = '" + userTypeString + "' title='" + title + "'>" + desc + "<span class=" + arrowClass + "></span></a>";
            }

            return link;
        }

        public static SelectList ToSelectList(this Enum enumObj)
        {
            var list = (from Enum d in Enum.GetValues(enumObj.GetType())
                        select new { ID = (int)Enum.Parse(enumObj.GetType(), Enum.GetName(enumObj.GetType(), d)), Name = d.ToString() }).ToList();
            return new SelectList(list, "ID", "Name");
        }
    }
}