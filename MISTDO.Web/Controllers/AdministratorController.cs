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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using OfficeOpenXml;
using System.Xml.Linq;
using OfficeOpenXml.Style;
using System.Net.Http.Headers;
using MISTDO.Web.Models.AdminViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MISTDO.Web.Controllers
{ 
    public class AdministratorController : Controller
    {
        public ITrainerService _trainer { get; }

        private readonly AdminApplicationDbContext admindbcontext;
        private readonly ApplicationDbContext dbcontext;
        private readonly TraineeApplicationDbContext Traineedbcontext;
        private readonly IHostingEnvironment _env;


        //user managers
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly UserManager<TraineeApplicationUser> _traineeuserManager;

        public AdministratorController(ITrainerService trainer, UserManager<ApplicationUser> userManager, IHostingEnvironment env, UserManager<TraineeApplicationUser> traineeuserManager, ApplicationDbContext context, TraineeApplicationDbContext traineedbcontext, AdminApplicationDbContext _admindbcontext)
        {
            _usermanager = userManager;
            _traineeuserManager = traineeuserManager;
            _trainer = trainer;
            admindbcontext = _admindbcontext;
            dbcontext = context;
            Traineedbcontext = traineedbcontext;
            _env = env;
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

            var owners = new List<TraineeApplicationUser>();
            foreach (var item in certs)
            {
                var user = await _traineeuserManager.FindByIdAsync(item.Owner);
                owners.Add(user);
            }
            ViewBag.Owners = owners;
            return View(certs.ToList());
        }
     
        public async Task<IActionResult> Account()
        {
            var allModuletrainee = await _trainer.GetAllModuleTrainees();
            var modules = new List<Modules>();
            foreach (var item in allModuletrainee)
            {
                modules.Add(admindbcontext.Modules.FirstOrDefault(m => m.Id.ToString() == item.ModuleId));
            }

            //var modules = await _trainer.GetAllModules();
                 ViewBag.modulecosts = modules;

            double total = modules.Sum(item => item.Cost);
            string cost = string.Format(new System.Globalization.CultureInfo("en-NG"), "{0:C2}", total);
            ViewBag.totalcost = cost;

            return View(allModuletrainee);

        }

        //private async List<Training> GetTraining()
        //{
        //    var modules = await _trainer.GetTrainee();
        //    List<Training> training = new List<Training>();
        //    foreach(var item in modules)
        //        training.Add(new Training { Id = item.Id, ModuleId = item.ModuleId, TrainingCentreId = item.TrainingCentreId, CertificateId = item.CertificateId, DateCreated = item.DateCreated });

        //    return training;
        //}
        //private List<Training> GetModeules()
        //{
        //    List<Modules> module = new List<Modules>();

        //    return module;
        //}


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
           var trainer = await dbcontext.Users.SingleOrDefaultAsync(m => m.Id == calenders.TrainingCentreId);
            ViewBag.center = trainer.CentreName;
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
        public async Task<IActionResult> DetailsModule(int? id)
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
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DetailsTrainingCenter(string id)
        {
           
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
            var notify = await dbcontext.Notifications.ToListAsync();
           
             ViewBag.Message = await dbcontext.Notifications.ToListAsync();
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
                    NotificationDateTime = DateTime.Now


                };

                dbcontext.Add(notify);

                await dbcontext.SaveChangesAsync();
                return RedirectToAction(nameof(TrainersNotification));
            }
            return View(notification);
        }

        public async Task<IActionResult> TraineesNotification()
        {
            var notify = await Traineedbcontext.Notifications.ToListAsync();

            ViewBag.Message = await Traineedbcontext.Notifications.ToListAsync();
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
                    NotificationDateTime = DateTime.Now


                };

                Traineedbcontext.Add(notify);

                await Traineedbcontext.SaveChangesAsync();
                return RedirectToAction(nameof(TraineesNotification));
            }
            return View(notification);
        }
        public async Task<IActionResult> ViewCertificate(string traineeid, string moduleid, string TrainingCentreId, int TrainingId)
        {
            var training = dbcontext.Trainings.FirstOrDefault(t => t.TraineeId == traineeid && t.ModuleId == moduleid && t.TrainingCentreId == TrainingCentreId);
            var centre = await _usermanager.FindByIdAsync(TrainingCentreId);

            var trainee = await _traineeuserManager.FindByIdAsync(traineeid);
            var module = admindbcontext.Modules.FirstOrDefault(m => m.Id == int.Parse(moduleid));


            ViewBag.Trainee = trainee;
            ViewBag.Centre = centre;
            ViewBag.Module = module;

            ViewBag.Training = training;



            return View();
        }
        public async Task<IActionResult> Feedback()
        {
            var feedback = await Traineedbcontext.Feedbacks.ToListAsync();
            return View(feedback);
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
        [HttpGet]
        public async Task<IActionResult> EditModule(int id)
        {


            var modules = await admindbcontext.Modules.SingleOrDefaultAsync(m => m.Id == id);
            if (modules == null)
            {
                return NotFound();
            }
            return View(modules);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModule(int id, Modules modules)
        {
            var user = await _usermanager.GetUserAsync(User);
            // id = user.Id;
            modules.Id = id;
            if (id != modules.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var train = new Modules()
                    {

                        Id = modules.Id,
                        Cost = modules.Cost,
                        Description = modules.Description,
                        ShortCode = modules.ShortCode,
                        CertificateCost = modules.CertificateCost,
                        Name = modules.Name


                    };
                    admindbcontext.Update(train);
                    await admindbcontext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleExists(modules.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AllModules));
            }
            return View(modules);
        }

        private bool ModuleExists(int id)
        {
            return admindbcontext.Modules.Any(e => e.Id == id);
        }

        public async Task<IActionResult> DeleteModule(int id)
        {
            

            var modules = await admindbcontext.Modules.SingleOrDefaultAsync(m => m.Id == id);
            if (modules == null)
            {
                return NotFound();
            }

            return View(modules);
        }

        // POST: Training/Delete/5
        [HttpPost, ActionName("DeleteModule")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var modules = await admindbcontext.Modules.SingleOrDefaultAsync(m => m.Id == id);
            admindbcontext.Modules.Remove(modules);
            await admindbcontext.SaveChangesAsync();
            return RedirectToAction(nameof(AllModules));
        }

        [HttpGet]
        public async Task<IActionResult> SupportUpdate(int id)
        {
           

            var support= await dbcontext.TrainerSupports.SingleOrDefaultAsync(m => m.SupportId == id);
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
            support.SupportId = id;
            if (id != support.SupportId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var train = new Support()
                    {
                        
                        SupportId = support.SupportId,
                        Subject = support.Subject,
                        Issue = support.Issue,
                        Response = support.Response,
                        SupportTimeStamp = DateTime.Now,
                        ResponseTimeStamp = DateTime.Now


                    };
                    dbcontext.Update(train);
                    await dbcontext.SaveChangesAsync();
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
                return RedirectToAction(nameof(Support));
            }
            return View(support);
        }

        private bool SupportExists(int id)
        {
            return dbcontext.TrainerSupports.Any(e => e.SupportId == id);
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
        [HttpGet]
        [Route("ExportCenters")]
        public IActionResult ExportCenters()
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

                IList<ApplicationUser> traineeList = dbcontext.Users.ToList();

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("User");
                using (var cells = worksheet.Cells[1, 1, 1, 10]) //(1,1) (1,5)
                {
                    cells.Style.Font.Bold = true;
                }

                int totalRows = traineeList.Count();

                worksheet.Cells[1, 1].Value = "Training Center ID";
                worksheet.Cells[1, 2].Value = "Training Center Name";
                worksheet.Cells[1, 3].Value = "Email";
                worksheet.Cells[1, 4].Value = "Address";
                worksheet.Cells[1, 5].Value = "Phone NUmber";

                int i = 0;
                for (int row = 2; row <= totalRows + 1; row++)
                {
                    worksheet.Cells[row, 1].Value = traineeList[i].Id;
                    worksheet.Cells[row, 2].Value = traineeList[i].CentreName;
                    worksheet.Cells[row, 3].Value = traineeList[i].Email;
                    worksheet.Cells[row, 4].Value = traineeList[i].CompanyAddress;
                    worksheet.Cells[row, 5].Value = traineeList[i].PhoneNumber;

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
        [Route("ExportModules")]
        public IActionResult ExportModules()
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

                IList<Modules> traineeList = admindbcontext.Modules.ToList();

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("User");
                using (var cells = worksheet.Cells[1, 1, 1, 10]) //(1,1) (1,5)
                {
                    cells.Style.Font.Bold = true;
                }

                int totalRows = traineeList.Count();

                worksheet.Cells[1, 1].Value = "Module ID";
                worksheet.Cells[1, 2].Value = "Module Name";
                worksheet.Cells[1, 3].Value = "Module Description";
                worksheet.Cells[1, 4].Value = "Module Registration Cost";
                worksheet.Cells[1, 5].Value = "Module Code";
               
                int i = 0;
                for (int row = 2; row <= totalRows + 1; row++)
                {
                    worksheet.Cells[row, 1].Value = traineeList[i].Id;
                    worksheet.Cells[row, 2].Value = traineeList[i].Name;
                    worksheet.Cells[row, 3].Value = traineeList[i].Description;
                    worksheet.Cells[row, 4].Value = traineeList[i].Cost;
                    worksheet.Cells[row, 5].Value = traineeList[i].ShortCode;
                   
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
        [Route("ExportCertificates")]
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

                IList<Certificate> certs= dbcontext.Certificates.ToList();
              

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
        [Route("ExportCustomer")]
        public IActionResult ExportTrainees()
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

        [HttpGet]
        [Route("ExportModuleTrainees")]
        public async Task<IActionResult> ExportModuleTrainees()
        {
            string rootFolder = _env.WebRootPath;
            string fileName = @"ExportModuleTrainees.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);

            FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(rootFolder, fileName));
            }

            using (ExcelPackage package = new ExcelPackage(file))
            {

                
                IList<Training> trainingList = dbcontext.Trainings.ToList();

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Trainings");
                using (var cells = worksheet.Cells[1, 1, 1, 10]) //(1,1) (1,5)
                {
                    cells.Style.Font.Bold = true;
                }
                int totalRows = trainingList.Count();


                worksheet.Cells[1, 1].Value = "Trainee ID";
                worksheet.Cells[1, 2].Value = "Training Name";
                worksheet.Cells[1, 3].Value = "Training Centre ID";
                worksheet.Cells[1, 4].Value = "Certificate ID";
                worksheet.Cells[1, 5].Value = "Date Registered";
                worksheet.Cells[1, 6].Value = "Training Start Date";
                worksheet.Cells[1, 7].Value = "Training End Date";
                worksheet.Cells[1, 8].Value = "Certifcate Gen  Date";


                int i = 0;
                for (int row = 2; row <= totalRows + 1; row++)
                {
                    worksheet.Cells[row, 1].Value = trainingList[i].TraineeId;
                    worksheet.Cells[row, 2].Value = trainingList[i].TrainingName;
                    worksheet.Cells[row, 3].Value = trainingList[i].TrainingCentreId;
                    worksheet.Cells[row, 4].Value = trainingList[i].CertificateId;
                    worksheet.Cells[row, 5].Value = trainingList[i].DateCreated.ToString();
                    worksheet.Cells[row, 6].Value = trainingList[i].TrainingStartDate.Date.ToString();
                    worksheet.Cells[row, 7].Value = trainingList[i].TrainingEndDate.Date.ToString();
                    worksheet.Cells[row, 8].Value = trainingList[i].CertGenDate.Date.ToString();
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
        [Route("ExportTrainings")]
        public IActionResult ExportTrainings()
        {
            string rootFolder = _env.WebRootPath;
            string fileName = @"ExportTrainings.xlsx";
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

                IList<Training> trainingList = dbcontext.Trainings.ToList();

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Trainings");
                using (var cells = worksheet.Cells[1, 1, 1, 10]) //(1,1) (1,5)
                {
                    cells.Style.Font.Bold = true;
                }
                int totalRows = trainingList.Count();
                

                worksheet.Cells[1, 1].Value = "Training Name";
                worksheet.Cells[1, 2].Value = "Training Centre ID";
                worksheet.Cells[1, 3].Value = "Trainee ID";
                worksheet.Cells[1, 4].Value = "Training Start Date";
                worksheet.Cells[1, 5].Value = "Training End Date";
                worksheet.Cells[1, 6].Value = "Certifcate Gen  Date";

            
                int i = 0;
                for (int row = 2; row <= totalRows + 1; row++)
                {
                    worksheet.Cells[row, 1].Value = trainingList[i].TrainingName;
                    worksheet.Cells[row, 2].Value = trainingList[i].TrainingCentreId;
                    worksheet.Cells[row, 3].Value = trainingList[i].TraineeId;
                    worksheet.Cells[row, 4].Value = trainingList[i].TrainingStartDate.Date.ToString();
                    worksheet.Cells[row, 5].Value = trainingList[i].TrainingEndDate.Date.ToString();
                    worksheet.Cells[row, 6].Value = trainingList[i].CertGenDate.Date.ToString();
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
        public ActionResult DownloadTrainngFile()
        {
            string path = AppDomain.CurrentDomain.DynamicDirectory + "wwwroot/";
            byte[] fileBytes = System.IO.File.ReadAllBytes(path + "ExportTrainees.xlsx");
            string filename = "ExportTrainees.xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }


    }
}
