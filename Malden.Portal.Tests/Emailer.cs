using System;
using System.Net;
using System.Net.Mail;

namespace Malden.Portal.Tests
{
    public static class Emailer
    {
        public static void SendEmail()
        {
            String APIKey = "69f2ebd894cb69ccb1b6351c368d0250";
            String SecretKey = "a02230af3a89f3596df99c16de65baa1";
            String From = "ps@malden.co.uk";
            String To = "prabah@gmail.com";

            MailMessage msg = new MailMessage();

            msg.From = new MailAddress(From);

            msg.To.Add(new MailAddress(To));

            msg.Subject = "Your mail from Mailjet";
            msg.Body = "Your mail from Mailjet, sent by C#.";

            SmtpClient client = new SmtpClient("in.mailjet.com", 465);
            client.EnableSsl = true;

            client.Credentials = new NetworkCredential(APIKey, SecretKey);

            client.Send("ps@malden.co.uk", "prabah@gmail.com", "Test C#", "test c#");
        }
    }
}