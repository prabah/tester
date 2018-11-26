
using System.ComponentModel.DataAnnotations;
namespace Malden.Portal.BLL
{
    public class Distributor
    {
        public string Id { get; set; }

        [Required]
        public string Email { get; set; }
        
        public string Token { get; set; }

        [Display(Name="Registered?")]
        public bool IsRegistered { get; set; }

        [Display(Name = "Activated?")]
        public bool IsActivated { get; set; }
    }
}
