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

        public ITrainerService _trainer { get; }

        private readonly ApplicationDbContext dbcontext;
        private readonly TraineeApplicationDbContext Traineedbcontext;
        private readonly AdminApplicationDbContext Admindbcontext;

        public TrainerDashboardController(ITrainerService trainer, ApplicationDbContext context, TraineeApplicationDbContext traineedbcontext, AdminApplicationDbContext admindb, UserManager<ApplicationUser> userManager)
        {
            _usermanager = userManager;

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


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public IActionResult ViewCertificate()
        {
            return View();
        }
    }
}