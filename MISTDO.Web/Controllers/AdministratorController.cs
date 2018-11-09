using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MISTDO.Web.Data;
using MISTDO.Web.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
using MISTDO.Web.Models;
//using Microsoft.Azure.KeyVault.Models;

namespace MISTDO.Web.Controllers
{ 
    public class AdministratorController : Controller
    {
        public ITrainerService _trainer { get; }

        private readonly AdminApplicationDbContext admindbcontext;

        public AdministratorController(ITrainerService trainer, AdminApplicationDbContext _admindbcontext)
        {
            _trainer = trainer;
            admindbcontext = _admindbcontext;
        }

        // GET: /<controller>/
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Login()
        {
            // Clear the existing external cookie to ensure a clean login process
            // await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        public async Task<IActionResult> AllCertificate()
        {
            var certs = await _trainer.GetAllCertificates();

         
            return View(certs.ToList());
        }

        public async Task<IActionResult> AllRegisteredTrainees()
        {

            var allRegistredtrainee = await _trainer.GetTrainees();

            return View(allRegistredtrainee);
        }

        public async Task<IActionResult> AllModuleTrainees()
        {

            var allModuletrainee = await _trainer.GetAllModuleTrainees();

            return View(allModuletrainee);
        }

        public async Task<IActionResult> AllTrainingCenter()
        {

            var allTrainingCenter = await _trainer.GetAllTrainingCenters();

            return View(allTrainingCenter);
        }

        public async Task<IActionResult> AllModules()
        {

            var allModules = await _trainer.GetAllModules();

            return View(allModules);
        }

        public IActionResult AddNewModule()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CreateModule([Bind("Name,Description,Cost,ShortCode,CertificateCost")] Modules modules)
        {

            if (ModelState.IsValid)
            {
                var updatemodule = new Modules
                {
                    Name = modules.Name,
                    Description = modules.Description,
                    Cost = modules.Cost,
                    ShortCode = modules.ShortCode,
                    CertificateCost = modules.CertificateCost
                };

                admindbcontext.Add(updatemodule);
                await admindbcontext.SaveChangesAsync();
                return RedirectToAction(nameof(AllModules));
            }
            else
            {
                return Content("OperationFailed");
            }
        }

    }
}
