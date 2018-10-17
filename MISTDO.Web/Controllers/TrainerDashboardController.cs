using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MISTDO.Web.Data;
using MISTDO.Web.Models;
using MISTDO.Web.Models.AccountViewModels;
using MISTDO.Web.Services;
using MISTDO.Web.ViewModels;

namespace MISTDO.Web.Controllers
{
    [Authorize]
    public class TrainerDashboardController : Controller
    {
        public ITrainerService _trainer { get; }

        private readonly ApplicationDbContext dbcontext;

        public TrainerDashboardController(ITrainerService trainer, ApplicationDbContext context)
        {
            _trainer = trainer;
            dbcontext = context;
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

         public async Task<IActionResult> ViewTrainee(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainee = await dbcontext.Trainees
                .SingleOrDefaultAsync(m => m.TraineeId == id);
            if (trainee == null)
            {
                return NotFound();
            }

            return View(trainee);
        }
        // GET: Certificates/Create

      public async Task<IActionResult> NewCertificate()
        {
            var trainees = await _trainer.GetAllTrainees();
            var traineesList = new List<SelectListItem>();

            foreach (var item in trainees)
            {
                traineesList.Add(new SelectListItem { Text = item.Email, Value = item.TraineeId.ToString() });
            }

            ViewBag.trainees = traineesList;
            return View();
        }


        // public async Task<IActionResult> NewCertificate()
        // {
        //     var trainees = await _trainer.GetAllTrainees();
        //     var traineesList = new List<SelectListItem>();

        //     foreach (var item in trainees)
        //     {
        //         traineesList.Add(new SelectListItem { Text = item.Email, Value = item.TraineeId.ToString() });
        //     }

        //     ViewBag.trainees = traineesList;
        //     return View();
        // }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterTrainee()
        {
            
            return View("~/Views/TrainerDashboard/RegisterTrainee.cshtml");
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterTrainee(Trainee models)
        {
            if (ModelState.IsValid)
            {
                dbcontext.Add(models);
                await dbcontext.SaveChangesAsync();
              //  return View("~/Views/TrainerDashboard/Trainee.cshtml");
            }
            return RedirectToAction(nameof(Trainee));
        }

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> NewCertificate(NewCertificateViewModel model)
        // {
        //     var tt = dbcontext.Trainees.FirstOrDefault(t => t.TraineeId == model.TraineeId);
        //     //model.Certificate.DateGenerated = DateTime.Now;
        //     //model.Certificate.Owner = tt;
        //     if (ModelState.IsValid)
        //     {
        //         //dbcontext.Add(model.Certificate);
        //         //await dbcontext.SaveChangesAsync();

        //         return RedirectToAction(nameof(Payment), new { traineeid = model.TraineeId });
        //     }
        //     return View(model);
        // }
        public IActionResult Payment(int traineeid)
        {
            var trainee = dbcontext.Trainees.FirstOrDefault(t => t.TraineeId == traineeid);

            return View(trainee);
        }
        public IActionResult ViewCertificate()

        {

            return View();
        }
    }

}