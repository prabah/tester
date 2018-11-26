using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Malden.Portal.BLL
{
    public class User
    {
        public enum UserType
        {
            [Description("Administrator")]
            Admin = 1,

            [Description("Customer")]
            Customer = 2,

            [Description("Distributor")]
            Distributor = 3,

            [Description("Deleted ?")]
            Deleted = 4,

        }

        public User(string email, string password)
        {
            //if (string.IsNullOrEmpty(email)) throw new ArgumentNullException("email", "Please enter a valid Email");
            //if (string.IsNullOrEmpty(password)) throw new ArgumentNullException("password", "Password is a required feild");

            Email = email;
            Password = password;
        }

        public User()
        {
        }

        [Required]
        public string Email { get; set; }

        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        public string Company { get; set; }

        [Display(Name = "Type of User")]
        public UserType TypeOfUser { get; set; }

        //[Display(Name = "Admin ?")]
        //public bool IsAdmin { get; set; }

        [Display(Name = "Blocked ?")]
        public bool IsBlocked { get; set; }

        public string Id { get; set; }

        public int RegisteredProductsCounter { get; set; }

        public virtual void SetUserType()
        {
            TypeOfUser = UserType.Customer;
        }
    }
}