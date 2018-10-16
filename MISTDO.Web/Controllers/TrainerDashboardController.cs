using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MISTDO.Web.Models.AccountViewModels;
using MISTDO.Web.Services;

namespace MISTDO.Web.Controllers
{
   [Authorize]
    public class TrainerDashboardController : Controller
    {
        public ITrainerService _trainer { get; }

        public TrainerDashboardController(ITrainerService trainer)
        {
            _trainer = trainer;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Certificate()
        {
            var certs = await _trainer.GetAllCertificates();
            return View(certs);
        }
         

         public async Task<IActionResult> Trainee()
        {
            var train = await _trainer.GetAllTrainees();    
            return View(train);
        }

        [HttpGet]
        [AllowAnonymous]
         public IActionResult RegisterTrainee(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View("~/Views/TrainerDashboard/RegisterTrainee.cshtml");
        }

        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterTrainee(RegisterTraineeViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
              
            }

            // If we got this far, something failed, redisplay form
            return View("~/Views/TrainerDashboard/RegisterTrainee.cshtml", model);
        }


    }
}