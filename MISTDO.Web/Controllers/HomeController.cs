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


        public ApplicationDbContext dbcontext { get; }

        public HomeController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
             ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
           
            dbcontext = context;
        }
        public async Task<IActionResult> Index()
        {
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


        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterNow(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterNow(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {

                    UserName = model.Email,
                    Email = model.Email,

                    PhoneNumber = model.PhoneNumber,
                    CompanyAddress = model.CompanyAddress,
                    CompanyName = model.CompanyName,
                    UserAddress = model.UserAddress,
                    FirstName = model.FirstName,
                    LastName = model.LastName,

                    State = model.State,
                    City = model.City,
                    Country = model.Country,
                    DateRegistered = DateTime.Now.Date,



                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var centredetails = new TrainingCentre
                    {
                        CentreName = model.CentreName,
                        OGISPUserName = model.OGISPUserName,
                        OGISPId = model.OGISPId,
                        User = user


                    };
                    dbcontext.Add(centredetails);
                    dbcontext.SaveChanges();


                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    var response = _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);


                    return View("ConfirmMail");

                }


            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
