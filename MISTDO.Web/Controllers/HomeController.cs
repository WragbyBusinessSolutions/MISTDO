using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MISTDO.Web.Data;
using MISTDO.Web.Models;
using MISTDO.Web.Models.AccountViewModels;
using MISTDO.Web.Services;

namespace MISTDO.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext tdbcontext;


        public ApplicationDbContext dbcontext { get; }

        public HomeController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
             ApplicationDbContext context, ApplicationDbContext contexted)
        {
            tdbcontext = contexted;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
           
            dbcontext = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewData["Success"] = "Data was saved successfully.";
            return View(await dbcontext.Calenders.ToListAsync());
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Trainer()
        {

            return View();
        }

        public IActionResult ForgetPassword()
        {

            return View();
        }

        public async Task<IActionResult> Calender()
        {
            return View(await tdbcontext.Calenders.ToListAsync());
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterNow(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
