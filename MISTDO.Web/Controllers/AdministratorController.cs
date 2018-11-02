using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MISTDO.Web.Data;
using MISTDO.Web.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
using MISTDO.Web.Models;

namespace MISTDO.Web.Controllers
{
    public class AdministratorController : Controller
    {
        public ITrainerService _trainer { get; }

        //private readonly TraineeApplicationDbContext Traineedbcontext;

        public AdministratorController(ITrainerService trainer, TraineeApplicationDbContext traineedbcontext)
        {
            _trainer = trainer;
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

    }
}
