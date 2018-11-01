using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MISTDO.Web.Data;
using Microsoft.EntityFrameworkCore;

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
        private readonly TraineeApplicationDbContext Traineedbcontext;

        public TrainerDashboardController(ITrainerService trainer, ApplicationDbContext context, TraineeApplicationDbContext traineedbcontext)
        {
            _trainer = trainer;
            dbcontext = context;
            Traineedbcontext = traineedbcontext;
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
     
        // GET: Certificates/Create
        [HttpGet]

        public async Task<IActionResult> NewCertificate(string TrainingCentreId, string ModuleId)
        {
            var trainings = await _trainer.GetNullCertificateTrainees(TrainingCentreId, ModuleId);
            var traineesList = new List<SelectListItem>();



            foreach (var item in trainings)
            {
                var Trainees = await Traineedbcontext.Users.Where(u => u.Id == item.TraineeId).ToListAsync();
                foreach (var trainee in Trainees)

                    traineesList.Add(new SelectListItem { Text = trainee.UserName, Value = trainee.Id });
            }

            ViewBag.trainees = traineesList;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewCertificate(NewCertificateViewModel model)
        {


            var tt = Traineedbcontext.Users.FirstOrDefault(t => t.Id == model.TraineeId);
            //model.Certificate.DateGenerated = DateTime.Now;
            //model.Certificate.Owner = tt;
            if (ModelState.IsValid)
            {
                //dbcontext.Add(model.Certificate);
                //await dbcontext.SaveChangesAsync();

                return RedirectToAction(nameof(Payment), new { traineeid = model.TraineeId });
            }
            return View(model);
        }
        public async Task<IActionResult> Payment(string traineeid)
        {
            var trainee = Traineedbcontext.Users.FirstOrDefault(t => t.Id == traineeid);
            var TraineeViewModel = new TraineeViewModel
            {
                FirstName = trainee.FirstName,
                LastName = trainee.LastName,
                Email = trainee.Email,
                PhoneNumber = trainee.PhoneNumber,
                CompanyName = trainee.CompanyName,
                CompanyAddress = trainee.CompanyAddress,
                UserAddress = trainee.UserAddress,
                TraineeId = trainee.Id
            };
            return View(trainee);
        }
        public IActionResult ViewCertificate()
        {
            return View();
        }
    }
}