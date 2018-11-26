using System;
using System.ComponentModel.DataAnnotations;
namespace Malden.Portal.BLL
{
    public class History
    {
        [Display(Name="Email")]
        public string UserEmail { get; set; }

        [Display(Name="Serial Number")]
        public int SerialNumber { get; set; }

        [Display(Name="Version")]
        public Version Version { get; set; }

        [Display(Name="Date & Time")]
        public DateTime TimeStamp { get; set; }

        [Display(Name="File Type")]
        public Malden.Portal.BLL.Release.ImageFileType ImageFileType { get; set; }

        [Display(Name = "Product")]
        public string Product { get; set; }

        [Display(Name = "User Type")]
        public Malden.Portal.BLL.User.UserType UserType { get; set; }

        [Display(Name = "Date & Time")]
        public string DateTimeStr { get; set; }

        public string ImageFileTypeStr { get; set; }
        public string UserTypeStr { get; set; }
        public string VersionStr { get; set; }
        public Release ReleaseDownloaded { get; set; }
    }
}