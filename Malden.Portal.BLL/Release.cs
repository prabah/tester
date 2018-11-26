using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Malden.Portal.BLL
{
    public class Release
    {
        public enum ImageFileType
        {
            [Description("Full Installer")]
            ISO = 1,

            [Description("Firmware")]
            Firmware = 2,

            [Description("Upgrade")]
            MSI = 3,

            [Description("Release Notes")]
            Notes = 4,
        }

        public Release()
        {
        }

        public Release(string version)
        {
            Version = new Version(version);
        }

        public string Id { get; set; }

        [Required]
        [Display(Name = "Date of Release ")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfRelease { get; set; }

        public Version Version { get; set; }

        public string ProductId { get; set; }

        public bool IsHidden { get; set; }

        public IList<CloudFile> ReleaseImageFiles { get; set; }
    }
}