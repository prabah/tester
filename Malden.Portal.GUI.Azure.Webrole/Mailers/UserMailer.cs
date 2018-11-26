using Mvc.Mailer;

namespace Malden.Portal.GUI.Azure.Webrole.Mailers
{ 
    public class UserMailer : MailerBase, IUserMailer 	
	{
		public UserMailer()
		{
			MasterName="_Layout";
		}

        public virtual MvcMailMessage Welcome(string name, string activationUrl, string recipient)
		{
			ViewBag.ActivationURL = activationUrl;
            ViewBag.Name = name;
			return Populate(x =>
			{
				x.Subject = "Welcome To Malden's Maintenance Portal";
				x.ViewName = "Welcome";
				x.To.Add(recipient);
			});
		}

        public virtual MvcMailMessage PasswordReset(string recipient, string activationUrl)
		{
            ViewBag.ActivationURL = activationUrl;
			return Populate(x =>
			{
				x.Subject = "Password Reset";
                x.ViewName = "ResetPassword";
				x.To.Add(recipient);
			});
           
		}
 	}
}