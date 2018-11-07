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
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace MISTDO.Web.Controllers
{
    [Authorize/*(AuthenticationSchemes = "TrainerAuth")*/]
    public class TrainerDashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly UserManager<TraineeApplicationUser> _traineeuserManager;
        private readonly IEmailSender _emailSender;

        public ITrainerService _trainer { get; }

        private readonly ApplicationDbContext dbcontext;
        private readonly TraineeApplicationDbContext Traineedbcontext;
        private readonly AdminApplicationDbContext Admindbcontext;
        private readonly IHostingEnvironment _env;
        private readonly IHostingEnvironment _envt;
        public IExcelToTrainingService _exceltoTraining { get; }
        public IExcelToTraineeService _exceltoTrainee { get; }


        public TrainerDashboardController(ITrainerService trainer, ApplicationDbContext context, TraineeApplicationDbContext traineedbcontext, AdminApplicationDbContext admindb, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, UserManager<TraineeApplicationUser> traineeuserManager,
             IHostingEnvironment env,  IExcelToTrainingService excelToTrainingService,  IExcelToTraineeService excelToTraineeService, IEmailSender emailSender )
        {
            _usermanager = userManager;
            _signInManager = signInManager;
            _traineeuserManager = traineeuserManager;
            _trainer = trainer;
            dbcontext = context;
            Traineedbcontext = traineedbcontext;
            Admindbcontext = admindb;
            _env = env;
            _exceltoTraining = excelToTrainingService;
            _emailSender = emailSender;
            _exceltoTrainee = excelToTraineeService;
            _envt = env;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Certificate()
        {
            var certs = await _trainer.GetAllCertificates();

            var owners = new List<TraineeApplicationUser>();
            foreach (var item in certs)
            {
                owners.Add(item.Owner);
            }
            ViewBag.Owners = owners;
            return View(certs.ToList());
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


        [HttpPost]

        public IActionResult EligibleUsersForCertificate(NewCertificateViewModel model)
        {
            if (model.TraineeId == null)
            {
                return Content("No Available Trainee");
            }
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
            Certificate certificate = new Certificate
            {
                CertNumber = updateTraining.CertificateId,
                CertStatus = "Valid",
                DateGenerated = updateTraining.CertGenDate,
                Owner = trainee,
                Trainer = centre,
                TrainerOrg = centre.CentreName,
                TrainerOrgId = centre.OGISPId

            };

            dbcontext.Add(certificate);

            await dbcontext.SaveChangesAsync();

            ViewBag.Training = updateTraining;



            return View();
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            //  _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public async Task<IActionResult> Training()
        {

            return View(await dbcontext.Calenders.ToListAsync());
        }

        public async Task<IActionResult> Notification()
        {
            var notify = await dbcontext.Notifications.ToListAsync();
            return View(notify);
        }
        public async Task<IActionResult> NotificationDetails(int? id)
        {
            if (id == null)
            {
                return View(await dbcontext.Notifications.ToListAsync());
            }

            var notification = await dbcontext.Notifications.SingleOrDefaultAsync(m => m.NotificationId == id);




            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        [HttpGet]
        //GET: Trainees/Support
        public IActionResult Support(int id)
        {


            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Support(Support support)
        {



            if (ModelState.IsValid)
            {


                dbcontext.Add(support);
                await dbcontext.SaveChangesAsync();
                return RedirectToAction(nameof(Support));
            }
            return View(support);

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Profile(string id)
        {
            var user = await _usermanager.GetUserAsync(User);
            id = user.Id;

            if (id == null)
            {
                return View(await dbcontext.Users.ToListAsync());
            }

            var trainer = await dbcontext.Users.SingleOrDefaultAsync(m => m.Id == id);




            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);

        }

        // GET: Trainee/Edit/5
        public async Task<IActionResult> EditProfile(string id)
        {
            var user = await _usermanager.GetUserAsync(User);
            id = user.Id;

            if (id == null)
            {
                return NotFound();
            }

            var trainer = await dbcontext.Users.SingleOrDefaultAsync(m => m.Id == id);
            if (trainer == null)
            {
                return NotFound();
            }
            return View(trainer);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(string id, ApplicationUser model)
        {
            var user = await _usermanager.GetUserAsync(User);
            id = user.Id;

            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                if (user != null)
                {
                    user.UserName = model.Email;
                    user.Email = model.Email;

                    user.PhoneNumber = model.PhoneNumber;
                    user.CompanyAddress = model.CompanyAddress;
                    user.CompanyName = model.CompanyName;
                    user.UserAddress = model.UserAddress;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;

                    user.State = model.State;
                    user.City = model.City;
                    user.Country = model.Country;
                    user.CentreName = model.CentreName;
                    user.OGISPUserName = model.OGISPUserName;
                    user.OGISPId = model.OGISPId;
                    user.EmailConfirmed = true;//Custom Column

                    var idResult = await _usermanager.UpdateAsync(user);//update
                }
                return RedirectToAction(nameof(Profile));
            }
            return View(model);

        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var tcentre = await _trainer.GetAllTrainingCenters();

            foreach (var item in tcentre)
                
                ViewBag.Tcenter = item.CentreName;

            var training = await dbcontext.Trainings
                .SingleOrDefaultAsync(m => m.Id == id);
            if (training == null)
            {
                return NotFound();
            }

            return View(training);
        }
        public async Task<IActionResult> DetailsTraining(int? id)
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
        [HttpGet]
        public async Task<IActionResult> EditTraining(int id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            var calenders = await dbcontext.Calenders.SingleOrDefaultAsync(m => m.Id == id);
            if (calenders == null)
            {
                return NotFound();
            }
            return View(calenders);
        }

        // POST: Training/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTraining(int id, Calender training)
        {
            var user = await _usermanager.GetUserAsync(User);
            // id = user.Id;
            if (id != training.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var train = new Calender()
                    {
                        Id = training.Id,
                        TrainingCentreId = user.Id,
                        Venue = training.Venue,
                        ModuleId = training.ModuleId,
                        TrainingStartDate = training.TrainingStartDate,
                        TraineeId = training.TraineeId,
                        Cost = training.Cost,
                        TrainingStartTime = training.TrainingStartTime,
                        TrainingEndTime = training.TrainingEndTime,
                        TrainingEndDate = training.TrainingEndDate,
                        TrainingName = training.TrainingName



                    };
                    dbcontext.Update(train);
                    await dbcontext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingExists(training.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Training));
            }
            return View(training);
        }

        public async Task<IActionResult> AttachTraineeTraining()
        {
            var modules = await _trainer.GetAllModules();

            var modulescost = new List<SelectListItem>();
            foreach (var item in modules)

                modulescost.Add(new SelectListItem { Text = item.Cost.ToString(), Value = item.Id.ToString() });

            var modulesList = new List<SelectListItem>();
           

            foreach (var item in modules)

                modulesList.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                
            ViewBag.modules = modulesList;

            ViewBag.modulecosts = modulescost;

            





            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> TraineeAttach(  RemitaResponse data, string TraineeId, string ModuleId, string ModuleCost, DateTime TrainingStartDate, DateTime TrainingEndDate)
        {


            var module = await _trainer.GetModulebyId(int.Parse(ModuleId));


            var user = await _usermanager.GetUserAsync(User);
            string id = user.Id;
            if (ModelState.IsValid)
            {
                var bae = await _trainer.GetModulebyId(int.Parse(ModuleId));
                var train = new Training()
                {

                    TrainingCentreId = user.Id,

                    ModuleId = ModuleId,
                    TrainingStartDate = TrainingStartDate,
                    TraineeId = TraineeId,
                    DateCreated = DateTime.Now,
                    TrainingEndDate = TrainingEndDate,
                    TrainingName = bae.Name



                };

                dbcontext.Add(train);

                await dbcontext.SaveChangesAsync();
                var url = Url.Action(nameof(Trainees));
                return Json(new { isSuccess = true, redirectUrl = url });
            }
            return View();
        }

        public async Task<IActionResult> CreateTraining()
        {
            var modules = await _trainer.GetAllModules();

            var modulesList = new List<SelectListItem>();
            foreach (var item in modules)

                modulesList.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            ViewBag.modules = modulesList;



            return View();
        }

        // POST: Training/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTraining( Calender calender, string id)
        {





            var user = await _usermanager.GetUserAsync(User);
            id = user.Id;
            if (ModelState.IsValid)
            {
                var bae = await _trainer.GetModulebyId(int.Parse(calender.ModuleId));

                var train = new Calender()
                {
                     

                    TrainingCentreId = user.Id,
                    Cost= calender.Cost,
                    ModuleId = calender.ModuleId,
                    TrainingStartDate = calender.TrainingStartDate,
                    TraineeId = calender.TraineeId,
                    TrainingEndTime = calender.TrainingEndTime,
                    TrainingStartTime = calender.TrainingStartTime,
                    Venue = calender.Venue,
                    TrainingEndDate = calender.TrainingEndDate,
                    TrainingName = bae.Name



                };

                dbcontext.Add(train);

                await dbcontext.SaveChangesAsync();
                return RedirectToAction(nameof(Training));
            }
            return View(calender);
        }
        public async Task<IActionResult> DeleteTraining(int id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            var calender = await dbcontext.Calenders.SingleOrDefaultAsync(m => m.Id == id);
            if (calender == null)
            {
                return NotFound();
            }

            return View(calender);
        }

        // POST: Training/Delete/5
        [HttpPost, ActionName("DeleteTraining")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var calenders = await dbcontext.Calenders.SingleOrDefaultAsync(m => m.Id == id);
            dbcontext.Calenders.Remove(calenders);
            await dbcontext.SaveChangesAsync();
            return RedirectToAction(nameof(Training));
        }

        private bool TrainingExists(int id)
        {
            return dbcontext.Calenders.Any(e => e.Id == id);
        }


        public async Task<IActionResult> TrainingUpload(Microsoft.AspNetCore.Http.IFormFile file)
        {


            if (file == null)
                return Content("Argument null");

            //var mimetype = MimeMapping.MimeTypes.GetMimeMapping(file.FileName);
            //if (mimetype != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            //{
            //    return Content("Invalid Content Type");

            //}
            var filePath = Path.Combine(_env.WebRootPath, ("productfiles\\" + file.Name));

            if (file.Length > 0)
            {
                if (System.IO.File.Exists(filePath))
                {

                    System.IO.File.Delete(filePath);
                }
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                await _exceltoTraining.ConvertFileToTrainingString(filePath);
            }
            return RedirectToAction("Index");

        }
        public ActionResult DownloadFile()
        {
            string path = AppDomain.CurrentDomain.DynamicDirectory + "wwwroot/templates/";
            byte[] fileBytes = System.IO.File.ReadAllBytes(path + "training.xlsx");
            string fileName = "training.xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
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

        //Tranees 
        public async Task<IActionResult> Trainees()
        {

            var trainings = await _trainer.GetTrainee();
            foreach (var item in trainings)
            {

                ViewBag.TrainerCenter = item.TrainingCentreId;
                ViewBag.TraineeId = item.TraineeId;
            }


            return View(await Traineedbcontext.Users.ToListAsync());
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
        public async Task<IActionResult> EditTrainees(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var trainee = await Traineedbcontext.Users.SingleOrDefaultAsync(m => m.Id == id);
            if (trainee == null)
            {
                return NotFound();
            }
            //var image = trainee.ImageUpload;

            //Stream stream = new MemoryStream(image);

            //IFormFile file = new FormFile(stream, 0, stream.Length, "Name", "FileName");


            return View(trainee);
        }

        // POST: Trainees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrainees(string id, TraineeViewModel trainee, Microsoft.AspNetCore.Http.IFormFile ImageUpload)
        {
            byte[] image = null;

            if (id != trainee.TraineeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (ImageUpload != null)

                {
                    if (ImageUpload.Length > 0)

                    //Convert Image to byte and save to database

                    {

                        byte[] p1 = null;
                        using (var fs1 = ImageUpload.OpenReadStream())
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                        image = p1;

                    }
                }
                try
                {


                    Traineedbcontext.Update(trainee);
                    await Traineedbcontext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TraineeExists(trainee.TraineeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Trainees));
            }
            return View(trainee);
        }

        private bool TraineeExists(string id)
        {
            return Traineedbcontext.Users.Any(e => e.Id == id);
        }

        public async Task<IActionResult> DeleteTrainees(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainee = await Traineedbcontext.Users.SingleOrDefaultAsync(m => m.Id == id);
            if (trainee == null)
            {
                return NotFound();
            }

            return View(trainee);
        }

        // POST: Trainees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var trainee = await Traineedbcontext.Users.SingleOrDefaultAsync(m => m.Id == id);
            Traineedbcontext.Users.Remove(trainee);
            await Traineedbcontext.SaveChangesAsync();
            return RedirectToAction(nameof(Trainees));
        }

        [HttpPost]
        public async Task<IActionResult> TraineeUpload(Microsoft.AspNetCore.Http.IFormFile file)
        {


            if (file == null)
                return Content("Argument null");

            //var mimetype = MimeMapping.MimeTypes.GetMimeMapping(file.FileName);
            //if (mimetype != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            //{
            //    return Content("Invalid Content Type");

            //}
            var filePath = Path.Combine(_envt.WebRootPath, ("traineefiles\\" + file.Name));

            if (file.Length > 0)
            {
                if (System.IO.File.Exists(filePath))
                {

                    System.IO.File.Delete(filePath);
                }
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                await _exceltoTrainee.ConvertFileToTraineeString(filePath);
            }
            return RedirectToAction(nameof(Trainees));


        }

        public ActionResult DownloadTraineeFile()
        {
            string path = AppDomain.CurrentDomain.DynamicDirectory + "wwwroot/templates/";
            byte[] fileBytes = System.IO.File.ReadAllBytes(path + "trainee.xlsx");
            string fileName = "trainee.xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public IActionResult CreateTrainees()
        {
            return View();
        }

        // POST: Trainees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTrainees(TraineeViewModel model)
        {
            if (ModelState.IsValid)
            {

                byte[] image = null;

                if (model.ImageUpload != null)

                {
                    byte[] p1 = null;
                    using (var fs1 = model.ImageUpload.OpenReadStream())
                    using (var ms1 = new MemoryStream())
                    {
                        fs1.CopyTo(ms1);
                        p1 = ms1.ToArray();
                    }
                    image = p1;

                }
                var user = new TraineeApplicationUser()
                {

                    UserName = model.Email,
                    Email = model.Email,

                    PhoneNumber = model.PhoneNumber,
                    CompanyAddress = model.CompanyAddress,
                    CompanyName = model.CompanyName,
                    UserAddress = model.UserAddress,
                    FirstName = model.FirstName,
                    LastName = model.LastName,

                    DateRegistered = DateTime.Now.Date,

                    ImageUpload = image

                };


                var result = await _traineeuserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var code = await _traineeuserManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    var response = _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);



                    return RedirectToAction(nameof(Trainees));

                }



            }
            return View();


        }
    }
}