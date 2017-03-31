using System.Net;
using System.Net.Mail;
using CollegeConnected.DataLayer;

namespace CollegeConnected.Services
{
    public class EmailService
    {
        public void SendEmailAsync(string emailTo, string mailbody, string subject)
        {
            var db = new UnitOfWork();
            var settings = db.SettingsRepository.GetUser();
            var from = new MailAddress($"{settings.EmailUsername}");
            var to = new MailAddress(emailTo);

            var useDefaultCredentials = true;
            var enableSsl = false;
            var replyto = $"{settings.EmailUsername}"; // set here your email; 
            var userName = string.Empty;
            var password = string.Empty;
            var port = 25;
            var host = "localhost";

            userName = $"{settings.EmailUsername}"; // setup here the username; 
            password = $"{settings.EmailPassword}"; // setup here the password; 
            bool.TryParse("false", out useDefaultCredentials); //setup here if it uses defaault credentials 
            bool.TryParse("true", out enableSsl); //setup here if it uses ssl 
            int.TryParse($"{settings.EmailPort}", out port); //setup here the port 
            host = $"{settings.EmailHostName}"; //setup here the host 

            using (var mail = new MailMessage(from, to))
            {
                mail.Subject = subject;
                mail.Body = mailbody;
                mail.IsBodyHtml = true;

                mail.ReplyToList.Add(new MailAddress(replyto, "My Email"));
                mail.ReplyToList.Add(from);
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.Delay |
                                                   DeliveryNotificationOptions.OnFailure |
                                                   DeliveryNotificationOptions.OnSuccess;

                using (var client = new SmtpClient())
                {
                    client.Host = host;
                    client.EnableSsl = enableSsl;
                    client.Port = port;
                    client.UseDefaultCredentials = useDefaultCredentials;

                    if (!client.UseDefaultCredentials && !string.IsNullOrEmpty(userName) &&
                        !string.IsNullOrEmpty(password))
                        client.Credentials = new NetworkCredential(userName, password);

                    /*await*/
                    client.Send(mail);
                }
            }

            //return true;
        }
    }
}