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
using OfficeOpenXml;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Text.Encodings.Web;

//FingerPrint Assembly 



namespace MISTDO.Web.Controllers
{
    [Authorize/*(AuthenticationSchemes = "TrainerAuth")*/]
    public class TrainerDashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly UserManager<TraineeApplicationUser> _traineeuserManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public ITrainerService _trainer { get; }

        private readonly ApplicationDbContext dbcontext;
        private readonly TraineeApplicationDbContext Traineedbcontext;
        private readonly AdminApplicationDbContext Admindbcontext;
        private readonly IHostingEnvironment _env;
        private readonly IHostingEnvironment _envt;
        public IExcelToTrainingService _exceltoTraining { get; }
        public IExcelToTraineeService _exceltoTrainee { get; }


        public TrainerDashboardController(ITrainerService trainer, ApplicationDbContext context, TraineeApplicationDbContext traineedbcontext, AdminApplicationDbContext admindb, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,  UserManager<TraineeApplicationUser> traineeuserManager,
             IHostingEnvironment env,  IExcelToTrainingService excelToTrainingService,  IExcelToTraineeService excelToTraineeService, IEmailSender emailSender )
        {
            _usermanager = userManager;
            _traineeuserManager = traineeuserManager;
            _signInManager = signInManager;
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
            ViewBag.Message = ViewBag.msg;
            //var trainings = dbcontext.Trainings.Where(t => t.Id == '1').Count();
            //ViewBag.centers = trainings;
            return View();
        }
        public async Task<IActionResult> Certificate()
        {
            var certs = await _trainer.GetAllCertificates();
            var center = await _usermanager.GetUserAsync(User);

            var owners = new List<TraineeApplicationUser>();

            foreach (var item in certs)
            {
                var user = await _traineeuserManager.FindByIdAsync(item.Owner);
                owners.Add(user);
            }
            ViewBag.Owners = owners;
            return View(certs.Where(t=>t.Trainer.Id == center.Id).ToList());
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
            var user = await _usermanager.GetUserAsync(User);
            var train = dbcontext.Trainings.ToList();
            var training =  train.FirstOrDefault(t => t.TraineeId == traineeid.Trim() & t.ModuleId == moduleid & t.TrainingCentreId == user.Id);
   
             var centre = await _usermanager.FindByIdAsync(user.Id);

           var  trainee = await _traineeuserManager.FindByIdAsync(traineeid);
            var module = Admindbcontext.Modules.FirstOrDefault(m => m.Id == int.Parse(moduleid));

            if (training.CertificateId != null)
            {
                return Content("Cert already Generated");
            }

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
                TrainingStartDate = training.TrainingStartDate,

            };

            var local = dbcontext.Set<Training>()
    .Local
    .FirstOrDefault(entry => entry.Id == updateTraining.Id);

            // check if local is not null 
            if (local != null) // I'm using a extension method
            {
                // detach
                dbcontext.Entry(local).State = EntityState.Detached;
            }
            // set Modified flag in your entry
            dbcontext.Entry(updateTraining).State = EntityState.Modified;

            dbcontext.SaveChanges();
            // save 
            Certificate certificate = new Certificate
            {
                CertNumber = updateTraining.CertificateId,
                CertStatus = "Valid",
                DateGenerated = updateTraining.CertGenDate,
                Owner = traineeid,
                Trainer = centre,
                TrainerOrg = centre.CentreName,
                TrainerOrgId = user.Id,
                ModuleId = int.Parse(moduleid),
                TrainingId = updateTraining.Id
                //  Course = module,
                //  Training = training

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
           // return RedirectToAction(nameof(HomeController.Index), "Home");
            return RedirectToAction(nameof(TrainerController.Login), "Trainer");
        }

        public async Task<IActionResult> Training()
        {

            return View(await dbcontext.Calenders.OrderByDescending(t => t.TrainingStartDate).ThenBy(d => d.TrainingStartDate.Day).ToListAsync());
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
        public async Task<IActionResult> Support(int id)
        {
            var user = await _usermanager.GetUserAsync(User);
            ViewBag.us = "user";
            ViewBag.res = "No Response";
            ViewBag.Message = await dbcontext.TrainerSupports.Where(t=>t.OwnerId == user.Id).ToListAsync();

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Support(Support support)
        {
            var user = await _usermanager.GetUserAsync(User);


            if (ModelState.IsValid)
            {

                //var suport = new Support()
                //{

                //    Subject = support.Subject,
                //    Issue = support.Issue,

                //    SupportTimeStamp = DateTime.Now,
                //    ResponseTimeStamp =DateTime.UtcNow




                //};
                support.SubjectId = "---";
                support.OwnerId = user.Id;
                support.Response = "---";
                support.SupportTimeStamp = DateTime.Now;
                dbcontext.Add(support);
                await dbcontext.SaveChangesAsync();
                return RedirectToAction(nameof(Support));
            }
            return View(support);

        }


        public async Task<IActionResult> SupportTrainees()
        {
            var support = await Traineedbcontext.TraineeSupports.ToListAsync();
            return View(support);
        }
        public async Task<IActionResult> SupportDetails(int? id)
        {
            if (id == null)
            {
                return View(await Traineedbcontext.TraineeSupports.ToListAsync());
            }

            var support = await Traineedbcontext.TraineeSupports.SingleOrDefaultAsync(m => m.SupportId == id);




            if (support == null)
            {
                return NotFound();
            }

            return View(support);
        }

        [HttpGet]
        public async Task<IActionResult> SupportUpdate(int id)
        {


            var support = await Traineedbcontext.TraineeSupports.SingleOrDefaultAsync(m => m.SupportId == id);
            if (support == null)
            {
                return NotFound();
            }
            return View(support);
        }

        // POST: support/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupportUpdate(int id, Support support)
        {
            var user = await _usermanager.GetUserAsync(User);
            // id = user.Id;
           
            if (id != support.SupportId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    support.ResponseTimeStamp = DateTime.Now;
                    support.SupportTimeStamp = DateTime.Now;
                    support.Response = support.Response;
                    Traineedbcontext.Update(support);
                    await Traineedbcontext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupportExists(support.SupportId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(SupportTrainees));
            }
            return View(support);
        }
        private bool SupportExists(int id)
        {
            return Traineedbcontext.TraineeSupports.Any(e => e.SupportId == id);
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
                    //user.UserAddress = model.UserAddress;
                    //user.FirstName = model.FirstName;
                    //user.LastName = model.LastName;

                    user.State = model.State;
                    user.City = model.City;
                    user.CentreName = model.CentreName;
                    user.CentreAddress = model.CentreAddress;
                    user.PermitNumber = model.PermitNumber;
                    user.LicenseExpDate = model.LicenseExpDate;
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
                        
                        TrainingStartDate = training.TrainingStartDate,
                       // TraineeId = training.TraineeId,
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
        public async Task<IActionResult> CompleteTraineeRegistration()

        {


            //var modules = await _trainer.GetAllModules();

            //var modulescost = new List<SelectListItem>();
            //foreach (var item in modules)

            //    modulescost.Add(new SelectListItem { Text = item.Cost.ToString(), Value = item.Id.ToString() });

            //var modulesList = new List<SelectListItem>();


            //foreach (var item in modules)

            //    modulesList.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });


            //ViewBag.modules = modulesList;

            //ViewBag.modulecosts = modulescost;


            //return View();

          
          
                try
                {
                    Process.Start(@"C:\Users\Femi Ogunyomi\Desktop\misdobio\MistdoBio.exe", "readme.txt"); // load training Centers application  file path


                }
                catch (Exception ex)
                {
                return View();
            }

                return RedirectToAction(nameof(Trainees));
           
          
        }
        public async Task<IActionResult> ViewTraineeCertificate(string traineeid, string moduleid, string TrainingCentreId, int TrainingId)
        {
            var training = dbcontext.Trainings.FirstOrDefault(t => t.TraineeId == traineeid && t.ModuleId == moduleid && t.TrainingCentreId == TrainingCentreId);
            var centre = await _usermanager.FindByIdAsync(TrainingCentreId);

            var trainee = await _traineeuserManager.FindByIdAsync(traineeid);
            var module = Admindbcontext.Modules.FirstOrDefault(m => m.Id == int.Parse(moduleid));


            ViewBag.Trainee = trainee;
            ViewBag.Centre = centre;
            ViewBag.Module = module;

            ViewBag.Training = training;



            return View();
        }

        public async Task<IActionResult> AttachTraineeTraining()
        {
            var modules = await _trainer.GetAllModules();

            var modulescost = new List<SelectListItem>();
            foreach (var item in modules)

                modulescost.Add(new SelectListItem { Text = item.Cost.ToString(), Value = item.Id.ToString() });

            var modulesList = new List<SelectListItem>();
           

            foreach (var item in modules)

                modulesList.Add(new SelectListItem { Text = item.Name , Value = item.Id.ToString() });

                
            ViewBag.modules = modulesList;

            ViewBag.modulecosts = modulescost;

           // ViewBag.Message = await _traineeuserManager.FindByIdAsync(TraineeId);



            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> TraineeAttach(/*  RemitaResponse data,*/ string TraineeId, string ModuleId, string ModuleCost, DateTime TrainingStartDate, DateTime TrainingEndDate)
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
                    TraineeId = TraineeId.Trim(),
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
                    
                    TrainingEndTime = calender.TrainingEndTime,
                    TrainingStartTime = calender.TrainingStartTime,
                    Venue = calender.Venue,
                    TrainingEndDate = calender.TrainingEndDate,
                    TrainingName = bae.Name,
                   //TraineeId = user.Id



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
            return RedirectToAction("Training");

        }
        public ActionResult DownloadFile()
        {
            string path = AppDomain.CurrentDomain.DynamicDirectory + "wwwroot/templates/";
            byte[] fileBytes = System.IO.File.ReadAllBytes(path + "training.xlsx");
            string fileName = "training.xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        public IActionResult PaymentCompleted(/*RemitaResponse data,*/ string traineeid, string moduleid)
        {
            //var training = new Training
            //{
            //    PaymentRefId = data.PaymentReference,
                

            //};

            //  var trainee = _traineeuserManager.FindByIdAsync(traineeid);
            var url = Url.Action(nameof(ViewCertificate), new { traineeid, moduleid });
            return Json(new { isSuccess = true, redirectUrl = url});
          //  return RedirectToAction(nameof(ViewCertificate), new {  traineeid, moduleid });
        }

        //Tranees 
        public async Task<IActionResult> Trainees()
        {
            var user = await _usermanager.GetUserAsync(User);
            List<TraineeApplicationUser> Trainees = new List<TraineeApplicationUser>();

            var trainings = await _trainer.GetCentreTrainings(user.Id);

            foreach (var trainee in trainings)
            {
                var tra = Traineedbcontext.Trainees.Where(t => t.Id == trainee.TraineeId);
                Trainees.AddRange(tra);
                                                              
            }
            
            return View(Trainees.Distinct());


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
                    
               




                var results = await _traineeuserManager.CreateAsync(user, model.Password);
                if (results.Succeeded)
                {
                    //var code = await _traineeuserManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);

                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your email and Registration",
                    //  $"Your email has been registered. With username: '{model.Email}' .Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>link</a>");
                    //var code = await _traineeuserManager.GenerateEmailConfirmationTokenAsync(user);
                    //  var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    //  var response = _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);



                    return RedirectToAction(nameof(Trainees));

                }



            }
            return View();


        }

        [HttpGet]
        [Route("ExportCertificates1")]
        public async Task<IActionResult> ExportCertificates()
        {
            string rootFolder = _env.WebRootPath;
            string fileName = @"ExportCertificate.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);

            FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(rootFolder, fileName));
            }

            using (ExcelPackage package = new ExcelPackage(file))
            {

                var center = await _usermanager.GetUserAsync(User);
                IList<Certificate> certs = dbcontext.Certificates.Where(t => t.Trainer.Id == center.Id).ToList();


                var owners = new List<TraineeApplicationUser>();




                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("User");
                using (var cells = worksheet.Cells[1, 1, 1, 10]) //(1,1) (1,5)
                {
                    cells.Style.Font.Bold = true;
                }

                int totalRows = certs.Count();

                worksheet.Cells[1, 1].Value = "Full Name";
                worksheet.Cells[1, 2].Value = "Email";
                worksheet.Cells[1, 3].Value = "Certificate Number";
                worksheet.Cells[1, 4].Value = "Certificate";
                worksheet.Cells[1, 5].Value = "Date Generated";
                worksheet.Cells[1, 6].Value = "Training Center Name";

                int i = 0;
                for (int row = 2; row <= totalRows + 1; row++)
                {
                    foreach (var item in certs)
                    {
                        var user = await _traineeuserManager.FindByIdAsync(item.Owner);
                        owners.Add(user);

                        worksheet.Cells[row, 1].Value = owners[i].FirstName + " " + owners[i].LastName;
                        worksheet.Cells[row, 2].Value = owners[i].Email;
                        worksheet.Cells[row, 3].Value = certs[i].CertNumber;
                        worksheet.Cells[row, 4].Value = certs[i].CertStatus;

                        worksheet.Cells[row, 5].Value = certs[i].DateGenerated.ToString();
                        worksheet.Cells[row, 6].Value = certs[i].TrainerOrg.ToString();


                    }
                    i++;




                }

                package.Save();

            }

            var result = PhysicalFile(Path.Combine(rootFolder, fileName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            Response.Headers["Content-Disposition"] = new ContentDispositionHeaderValue("attachment")
            {
                FileName = file.Name
            }.ToString();

            return result;
        }

        [HttpGet]
        [Route("ExportCalenders1")]
        public IActionResult ExportCalender()
        {
            string rootFolder = _env.WebRootPath;
            string fileName = @"ExportCalendar.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);


            FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(rootFolder, fileName));
            }

            // byte[] result = null;
            using (ExcelPackage package = new ExcelPackage(file))
            {

                IList<Calender> trainingList = dbcontext.Calenders.ToList();

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Trainings");
                using (var cells = worksheet.Cells[1, 1, 1, 10]) //(1,1) (1,5)
                {
                    cells.Style.Font.Bold = true;
                }
                int totalRows = trainingList.Count();


                worksheet.Cells[1, 1].Value = "Training Name";
                worksheet.Cells[1, 2].Value = "Training Centre ID";
                worksheet.Cells[1, 3].Value = "Cost";
                worksheet.Cells[1, 4].Value = "Training Start Date";
                worksheet.Cells[1, 5].Value = "Training End Date";
                worksheet.Cells[1, 6].Value = "Training Start Date";
                worksheet.Cells[1, 7].Value = "Training End Date";
                worksheet.Cells[1, 8].Value = "Venue";


                int i = 0;
                for (int row = 2; row <= totalRows + 1; row++)
                {
                    worksheet.Cells[row, 1].Value = trainingList[i].TrainingName;
                    worksheet.Cells[row, 2].Value = trainingList[i].TrainingCentreId;
                    worksheet.Cells[row, 3].Value = trainingList[i].Cost;
                    worksheet.Cells[row, 4].Value = trainingList[i].TrainingStartTime.TimeOfDay.ToString();
                    worksheet.Cells[row, 5].Value = trainingList[i].TrainingEndTime.TimeOfDay.ToString();
                    worksheet.Cells[row, 6].Value = trainingList[i].TrainingStartDate.Date.ToString();
                    worksheet.Cells[row, 7].Value = trainingList[i].TrainingEndDate.Date.ToString();
                    worksheet.Cells[row, 8].Value = trainingList[i].Venue.ToString();
                    i++;
                }


                package.Save();

                //  result = package.GetAsByteArray();



            }

            //  return File(result, "application/vnd.ms-excel", fileName );

            var result = PhysicalFile(Path.Combine(rootFolder, fileName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            Response.Headers["Content-Disposition"] = new ContentDispositionHeaderValue("attachment")
            {
                FileName = file.Name
            }.ToString();

            return result;



        }
        [HttpGet]
        [Route("ExportCenterTrainee")]
        public IActionResult ExportCenterTrainees()
        {
            string rootFolder = _env.WebRootPath;
            string fileName = @"ExportTrainees.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);

            FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(rootFolder, fileName));
            }

            using (ExcelPackage package = new ExcelPackage(file))
            {

                IList<TraineeApplicationUser> traineeList = Traineedbcontext.Users.ToList();

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("User");
                using (var cells = worksheet.Cells[1, 1, 1, 10]) //(1,1) (1,5)
                {
                    cells.Style.Font.Bold = true;
                }

                int totalRows = traineeList.Count();

                worksheet.Cells[1, 1].Value = "First Name";
                worksheet.Cells[1, 2].Value = "Last Name";
                worksheet.Cells[1, 3].Value = "Trainee ID";
                worksheet.Cells[1, 4].Value = "Email";
                worksheet.Cells[1, 5].Value = "Company Name";
                worksheet.Cells[1, 6].Value = "Company Address";
                worksheet.Cells[1, 7].Value = "User Address";
                worksheet.Cells[1, 8].Value = "Registration Date";
                int i = 0;
                for (int row = 2; row <= totalRows + 1; row++)
                {
                    worksheet.Cells[row, 1].Value = traineeList[i].FirstName;
                    worksheet.Cells[row, 2].Value = traineeList[i].LastName;
                    worksheet.Cells[row, 3].Value = traineeList[i].Id;
                    worksheet.Cells[row, 4].Value = traineeList[i].Email;
                    worksheet.Cells[row, 5].Value = traineeList[i].CompanyName;
                    worksheet.Cells[row, 6].Value = traineeList[i].CompanyAddress;
                    worksheet.Cells[row, 7].Value = traineeList[i].CompanyAddress;
                    worksheet.Cells[row, 8].Value = traineeList[i].DateRegistered.ToString();
                    i++;
                }

                package.Save();

            }

            var result = PhysicalFile(Path.Combine(rootFolder, fileName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            Response.Headers["Content-Disposition"] = new ContentDispositionHeaderValue("attachment")
            {
                FileName = file.Name
            }.ToString();

            return result;
        }
    }
}