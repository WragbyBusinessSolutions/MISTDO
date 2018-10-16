using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
         public IActionResult Trainee()
        {
            return View();
        }
    }
}