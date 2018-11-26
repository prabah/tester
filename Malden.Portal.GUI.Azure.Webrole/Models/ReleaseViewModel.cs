using Malden.Portal.BLL;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Malden.Portal.GUI.Azure.Webrole.Models
{
    public class ReleaseViewModel
    {
        public byte[] File { get; set; }

        public Release Release { get; set; }

        public string Id { get; set; }

        [Required]
        [Display(Name = "Product")]
        public string ProductId { get; set; }

        public string ProductName { get; set; }

        [Required]
        [Display(Name = "Version ")]
        public string VersionString { get; set; }

        public IEnumerable<Product> Products { get; set; }

        [Display(Name = "Hide")]
        public bool IsHidden { get; set; }

        public IList<CloudFile> ImageFiles { get; set; }

        public IList<BlobManager> ImageFileCount { get; set; }
    }
}