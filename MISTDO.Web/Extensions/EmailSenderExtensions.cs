using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MISTDO.Web.Services;

namespace MISTDO.Web.Services
{
    public static class EmailSenderExtensions
    {
        public static string SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendLinkEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}
