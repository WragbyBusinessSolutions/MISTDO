using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MISTDO.Web.Models;
using MISTDO.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Services
{
   public  interface IMistdo
    {
        

        Task<bool> IsAccountActivatedAsync(string email, UserManager<ApplicationUser> userManager);

        Task SendEmailByGmailAsync(string fromEmail,
            string fromFullName,
            string subject,
            string messageBody,
            string toEmail,
            string toFullName,
            string smtpUser,
            string smtpPassword,
            string smtpHost,
            int smtpPort,
            bool smtpSSL);

        //Task CreateDefaultOrganization(string applicationUserId,
        //    ApplicationDbContext context);

        //Task<string> UploadFile(List<IFormFile> files, IHostingEnvironment env);
    }
}
