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
using Microsoft.AspNetCore.Identity;
using TraineeViewModel = MISTDO.Web.Models.AccountViewModels.TraineeViewModel;

namespace MISTDO.Web.Controllers
{
    [Authorize]
    public class TrainerDashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly UserManager<TraineeApplicationUser> _traineeuserManager;

        public ITrainerService _trainer { get; }

        private readonly ApplicationDbContext dbcontext;
        private readonly TraineeApplicationDbContext Traineedbcontext;
        private readonly AdminApplicationDbContext Admindbcontext;

        public TrainerDashboardController(ITrainerService trainer, ApplicationDbContext context, TraineeApplicationDbContext traineedbcontext, AdminApplicationDbContext admindb, UserManager<ApplicationUser> userManager, UserManager<TraineeApplicationUser> traineeuserManager)
        {
            _usermanager = userManager;
            _traineeuserManager = traineeuserManager;
            _trainer = trainer;
            dbcontext = context;
            Traineedbcontext = traineedbcontext;
            Admindbcontext = admindb;
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


        public async Task<IActionResult> Modules()
        {
            var modules = await _trainer.GetAllModules();

            var modulesList = new List<SelectListItem>();
            foreach (var item in modules)

                modulesList.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });


            ViewBag.modules = modulesList;
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Modules(Modules model)
        {

            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(EligibleUsersForCertificate), new { ModuleId = model.Id });
            }
            return View(model);
        }



        public async Task<IActionResult> EligibleUsersForCertificate(int ModuleId)
        {
            var users = new List<SelectListItem>();

            var Trainer = await _usermanager.GetUserAsync(User);


            //   var traineesList = new List<TraineeViewModel>();
            var trainings = await _trainer.GetNullCertificateTrainees(Trainer.Id, ModuleId.ToString());
            foreach (var trainee in trainings)
            {
                var TraineeApplicationUser = Traineedbcontext.Users.Where(t => t.Id == trainee.TraineeId).ToList();

                foreach (var user in TraineeApplicationUser)
                {
                    users.Add(new SelectListItem { Text = user.UserName, Value = user.Id });

                }

            }
            ViewBag.users = users;

            ViewBag.ModuleId = ModuleId;

            return View();


        }


      
        public IActionResult EligibleUsersForCertificate(NewCertificateViewModel model)
        {
            if (ModelState.IsValid)
            {


                return RedirectToAction(nameof(Payment), new { traineeid = model.TraineeId, moduleid = model.ModuleId });
            }
            return View(model);
        }



        public async Task<IActionResult> Payment(string traineeid, int moduleid)
        {
            var module = await _trainer.GetModulebyId(moduleid);
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
            ViewBag.Module = module;
            return View(TraineeViewModel);
        }
       
        public async Task<IActionResult> ViewCertificate(string traineeid, string moduleid)
        {
            var training = dbcontext.Trainings.FirstOrDefault(t => t.TraineeId == traineeid && t.ModuleId == moduleid);
                        var centre = await _usermanager.FindByIdAsync(training.TrainingCentreId);

              var  trainee = await _traineeuserManager.FindByIdAsync(traineeid);
            var module = Admindbcontext.Modules.FirstOrDefault(m => m.Id == int.Parse(moduleid));

            var CertId = Helpers.GetCertId.RandomString(5);

            ViewBag.Trainee = trainee;
            ViewBag.Centre = centre;
            ViewBag.Module = module;


            var updateTraining = new Training
            {
                CertGenDate = DateTime.Now,
                CertificateId = CertId,
                DateCreated = training.DateCreated,
                Id = training.Id,
                ModuleId = training.ModuleId,
                PaymentRefId = training.PaymentRefId,
                TraineeId = training.TraineeId,
                TrainingCentreId = training.TrainingCentreId,
                TrainingEndDate = training.TrainingEndDate,
                TrainingName = training.TrainingName,
                TrainingStartDate = training.TrainingStartDate
            };

            var local = dbcontext.Set<Training>()
    .Local
    .FirstOrDefault(entry => entry.Id.Equals(updateTraining.Id));

            // check if local is not null 
            if (local != null) // I'm using a extension method
            {
                // detach
                dbcontext.Entry(local).State = EntityState.Detached;
            }
            // set Modified flag in your entry
            dbcontext.Entry(updateTraining).State = EntityState.Modified;

            // save 
            

            await dbcontext.SaveChangesAsync();

            ViewBag.Training = updateTraining;



            return View();
        }
       
        public IActionResult PaymentCompleted(RemitaResponse data, string traineeid, string moduleid)
        {
            var training = new Training
            {
                PaymentRefId = data.PaymentReference,
                

            };

            //  var trainee = _traineeuserManager.FindByIdAsync(traineeid);
            var url = Url.Action(nameof(ViewCertificate), new { traineeid, moduleid });
            return Json(new { isSuccess = true, redirectUrl = url});
          //  return RedirectToAction(nameof(ViewCertificate), new {  traineeid, moduleid });
        }
    }
}