using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MISTDO.Web.Data;
using MISTDO.Web.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
using MISTDO.Web.Models;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.EntityFrameworkCore;

namespace MISTDO.Web.Controllers
{ 
    public class AdministratorController : Controller
    {
        public ITrainerService _trainer { get; }

        private readonly AdminApplicationDbContext admindbcontext;
        private readonly ApplicationDbContext dbcontext;
        private readonly TraineeApplicationDbContext Traineedbcontext;

        public AdministratorController(ITrainerService trainer, ApplicationDbContext context, TraineeApplicationDbContext traineedbcontext, AdminApplicationDbContext _admindbcontext)
        {
            _trainer = trainer;
            admindbcontext = _admindbcontext;
            dbcontext = context;
            Traineedbcontext = traineedbcontext;
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

        public async Task<IActionResult> AllCalender()
        {

            return View(await dbcontext.Calenders.ToListAsync());
        }

        public async Task<IActionResult> DetailsCalender(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calenders = await dbcontext.Calenders
                .SingleOrDefaultAsync(m => m.Id == id);
            if (calenders == null)
            {
                return NotFound();
            }

            return View(calenders);
        }
        public async Task<IActionResult> AllRegisteredTrainees()
        {

            var allRegistredtrainee = await _trainer.GetTrainees();

            return View(allRegistredtrainee);
        }

        public async Task<IActionResult> DetailsTrainees(string id)
        {

            var train = await dbcontext.Trainings.Where(t => t.TraineeId == id).ToListAsync();

            ViewBag.trainings = train;



            if (id == null)
            {
                return View(await Traineedbcontext.Users.ToListAsync());
            }

            var trainee = await Traineedbcontext.Users.SingleOrDefaultAsync(m => m.Id == id);




            if (trainee == null)
            {
                return NotFound();
            }

            return View(trainee);
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

        // GET: Training/Create
        public async Task<IActionResult> TrainersNotification()
        {
            
            return View();
        }

        // POST: Training/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TrainersNotification( Notification notification, string id)
        {
            if (ModelState.IsValid)
            {
                var notify = new Notification()
                {

                    NotificationMessage = notification.NotificationMessage,
                    NotificationDateTime = notification.NotificationDateTime


                };

                dbcontext.Add(notify);

                await dbcontext.SaveChangesAsync();
                return RedirectToAction(nameof(TrainersNotification));
            }
            return View(notification);
        }

        public async Task<IActionResult> TraineesNotification()
        {

            return View();
        }

        // POST: Training/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TraineesNotification(Notification notification, string id)
        {
            if (ModelState.IsValid)
            {
                var notify = new Notification()
                {

                    NotificationMessage = notification.NotificationMessage,
                    NotificationDateTime = notification.NotificationDateTime


                };

                Traineedbcontext.Add(notify);

                await Traineedbcontext.SaveChangesAsync();
                return RedirectToAction(nameof(TraineesNotification));
            }
            return View(notification);
        }

        public async Task<IActionResult> Support()
        {
            var support = await dbcontext.TrainerSupports.ToListAsync();
            return View(support);
        }
        public async Task<IActionResult> SupportDetails(int? id)
        {
            if (id == null)
            {
                return View(await dbcontext.TrainerSupports.ToListAsync());
            }

            var support = await dbcontext.TrainerSupports.SingleOrDefaultAsync(m => m.SupportId == id);




            if (support == null)
            {
                return NotFound();
            }

            return View(support);
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
