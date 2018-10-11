using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        string SendLinkEmailAsync(string emailAdd, string subject, string message);
        string SendPlainEmailAsync(string emailAdd, string subject, string message);
        string SendPlainEmailToGroupAsync(List<string> emailAdd, string subject, string message);
    }
}
