using Mvc.Mailer;

namespace Malden.Portal.GUI.Azure.Webrole.Mailers
{ 
    public interface IUserMailer
    {
        MvcMailMessage Welcome(string name, string activationUrl, string recipient);
        MvcMailMessage PasswordReset(string recipient, string activationUrl);
	}
}