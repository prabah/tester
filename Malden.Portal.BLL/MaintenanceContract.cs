using System;
using System.ComponentModel.DataAnnotations;

namespace Malden.Portal.BLL
{
    public class MaintenanceContract
    {
        public string Id { get; set; }

        [Display(Name = "Date of Expiry")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfExpiry { get; set; }

        [Display(Name = "Id")]
        public string SerialKeyId { get; set; }

        [Display(Name = "Serial Number")]
        public int SerialNumber { get; set; }

        public Product Product { get; set; }

        public int OrderId { get; set; }
    }
}