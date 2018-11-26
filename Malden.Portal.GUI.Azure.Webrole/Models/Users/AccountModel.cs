using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Malden.Portal.GUI.Azure.Webrole.Models.Users
{
    public class AccountModel : IValidatableObject
    {
        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Company:")]
        [DataType(DataType.Text)]
        public string Company { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Email:")]
        //Must be moved to project settings variable
        [RegularExpression(@"^([0-9a-zA-Z].*?@([0-9a-zA-Z].*\.\w{2,4}))$",
            ErrorMessage = "Invalid Email Address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage="Field is required")]
        [Display(Name = "Name:")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password:")]

        [RegularExpression("^(?=[\x21-\x7E]*[0-9])(?=[\x21-\x7E]*[A-Z])(?=[\x21-\x7E]*[a-z])(?=[\x21-\x7E]*[\x21-\x2F|\x3A-\x40|\x5B-\x60|\x7B-\x7E])[\x21-\x7E]{8,}$",
        ErrorMessage = "Password should be 8 characters minimum and must contain at least 1 uppercase, 1 lowercase, 1 number and 1 special character")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password:")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (validationContext == null || !validationContext.Items.ContainsKey("Context")) yield break;
            if (true)
            {
                yield return new ValidationResult("Unique", new[] { "Email" });
            }
        }
    }

    public class CustomLoginModel
    {
        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Email:")]
        [RegularExpression(@"^([0-9a-zA-Z].*?@([0-9a-zA-Z].*\.\w{2,4}))$",
            ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Field is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password:")]
        public string Password { get; set; }
    }


    public class PasswordResetModel
    {
        [Required(ErrorMessage = "Field is required")]
        [RegularExpression("^(?=[\x21-\x7E]*[0-9])(?=[\x21-\x7E]*[A-Z])(?=[\x21-\x7E]*[a-z])(?=[\x21-\x7E]*[\x21-\x2F|\x3A-\x40|\x5B-\x60|\x7B-\x7E])[\x21-\x7E]{8,}$",
           ErrorMessage = "Password should be 8 characters minimum and must contain at least 1 uppercase, 1 lowercase, 1 number and 1 special character")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password:")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (validationContext == null || !validationContext.Items.ContainsKey("Context")) yield break;
            if (true)
            {
                yield return new ValidationResult("Unique", new[] { "Email" });
            }
        }
    }


    public class EmailPasswordResetModel
    {
        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Email:")]
        [RegularExpression(@"^([0-9a-zA-Z].*?@([0-9a-zA-Z].*\.\w{2,4}))$",
            ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
    }


}