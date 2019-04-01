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
        private readonly TraineeApplicationDbContext Traineedbcontext;


        public ApplicationDbContext dbcontext { get; }

        public HomeController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender, TraineeApplicationDbContext traineedbcontext,
             ApplicationDbContext context, ApplicationDbContext contexted)
        {
            tdbcontext = contexted;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            Traineedbcontext = traineedbcontext;

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
        [HttpGet]
        public async Task<IActionResult> CertificateCheck()
        {
            ViewBag.Message = "";
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Certificate(string cert)
        {
            if (cert == null)
            {
                return NotFound();
            }

            var certificate = await tdbcontext.Certificates
                .SingleOrDefaultAsync(m => m.CertNumber.ToString() == cert);
            if (certificate == null)
            {
                ViewBag.Message = ViewBag.Message + "Certificate is not Valid";
                return View(nameof(CertificateCheck));
            }
            var trainee = await Traineedbcontext.Users.FirstOrDefaultAsync(a => a.Id == certificate.Owner);

            ViewBag.name = trainee.FirstName+" "+trainee.LastName;
            ViewBag.uid = trainee.UID;
            ViewBag.Message = ViewBag.Message + "Certificate is Valid";

         

            return View(nameof(CertificateCheck));
        }

        public IActionResult ForgetPassword()
        {

            return View();
        }

        public async Task<IActionResult> Calender()
        {
            return View(await tdbcontext.Calenders.ToListAsync());
        }
        public async Task<IActionResult> DetailsCalender(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calenders = await tdbcontext.Calenders
                .SingleOrDefaultAsync(m => m.Id == id);
            var trainer = await dbcontext.Users.SingleOrDefaultAsync(m => m.Id == calenders.TrainingCentreId);
            ViewBag.center = trainer.CentreName;
            if (calenders == null)
            {
                return NotFound();
            }

            return View(calenders);
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
