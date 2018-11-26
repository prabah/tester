using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Malden.Portal.BLL
{
    public class UserPurchase
    {
        public UserPurchase()
        { }

        public UserPurchase(string serialNumber, string userId)
        {
            if (string.IsNullOrEmpty(serialNumber) || string.IsNullOrWhiteSpace(serialNumber)) throw new ArgumentNullException("serialNumber", "Please enter a valid serial number");

            RegistrationCode = serialNumber;
            UserId = userId;
        }

        public Product Product { get; set; }

        [Required]
        [Display(Name = "Serial Number")]
        public string RegistrationCode { get; set; }

        public string Id { get; set; }

        public string PurchaseId { get; set; }

        public string ProductId { get; set; }

        public string UserId { get; set; }

        public int SerialNumber { get; set; }

        public string CurrentRelease { get; set; }

        public Release AvailableRelease { get; set; }

        //public IList<CloudFile> AvailableReleaseFiles { get; set; }

        public IList<Release> OldReleases { get; set; }

        public DateTime MaintenanceExpiryDate { get; set; }

        public bool IsMaintenanceAvailable { get; set; }

      }
}