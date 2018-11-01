using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MISTDO.Web.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        SmtpClient SmtpServer;
        string MailerResponse;
        public EmailSender()
        {

            SmtpClient smtpClient = new SmtpClient("smtp.office365.com");
            SmtpServer = smtpClient;
            SmtpServer.Port = 587;
            SmtpServer.EnableSsl = true;
            SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            MailerResponse = "";

        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask;
        }


        public string SendLinkEmailAsync(string emailAdd, string subject, string message)
        {
            SmtpServer.Credentials = new System.Net.NetworkCredential("cspaccount@wragbysolutions.com", "CSP@wbst@dm!n");
            try
            {
                MailMessage mailMessage = new MailMessage();
                MailMessage mail = mailMessage;
                mail.From = new MailAddress("cspaccount@wragbysolutions.com");
                mail.To.Add(emailAdd);
                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                SmtpServer.Send(mail);
                MailerResponse = "Success";
            }
            catch (Exception ex)
            {
                MailerResponse = "Failure";
                var ErrorMessage = ex.Message;
            }
            return MailerResponse;
        }

        public string SendPlainEmailAsync(string emailAdd, string subject, string message)
        {
            SmtpServer.Credentials = new System.Net.NetworkCredential("cspaccount@wragbysolutions.com", "CSP@wbst@dm!n");
            try
            {
                MailMessage mailMessage = new MailMessage();
                MailMessage mail = mailMessage;
                mail.From = new MailAddress("cspaccount@wragbysolutions.com");
                mail.To.Add(emailAdd);
                mail.Subject = subject;
                mail.Body = message;
                SmtpServer.Send(mail);
                MailerResponse = "Success";
            }
            catch (Exception ex)
            {
                MailerResponse = "Failure";
                var ErrorMessage = ex.Message;
            }
            return MailerResponse;
        }
        public string SendPlainEmailToGroupAsync(List<string> emailAdd, string subject, string message)
        {
            SmtpServer.Credentials = new System.Net.NetworkCredential("cspaccount@wragbysolutions.com", "CSP@wbst@dm!n");
            try
            {
                MailMessage mailMessage = new MailMessage();
                MailMessage mail = mailMessage;
                mail.From = new MailAddress("cspaccount@wragbysolutions.com");
                foreach (var item in emailAdd)
                {
                    mail.To.Add(item);


                }
                mail.Subject = subject;
                mail.Body = message;
                SmtpServer.Send(mail);
                MailerResponse = "Success";
            }
            catch (Exception ex)
            {
                MailerResponse = "Failure";
                var ErrorMessage = ex.Message;
            }
            return MailerResponse;
        }
    }
}
