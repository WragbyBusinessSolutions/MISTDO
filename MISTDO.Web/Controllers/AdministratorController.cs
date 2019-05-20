using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MISTDO.Web.Data;
using MISTDO.Web.Services;

using System.Drawing;
using System.IO;
using QRCoder;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
using MISTDO.Web.Models;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using System.Xml.Linq;
using OfficeOpenXml.Style;
using System.Net.Http.Headers;
using MISTDO.Web.Models.AdminViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using MISTDO.Web.Models.AccountViewModels;
using System.Net.Mail;
using System.Net;
using MISTDO.Web.ViewModels;

namespace MISTDO.Web.Controllers
{ 
    public class AdministratorController : Controller
    {
        public ITrainerService _trainer { get; }

        private readonly AdminApplicationDbContext admindbcontext;
        private readonly ApplicationDbContext dbcontext;
        private readonly TraineeApplicationDbContext Traineedbcontext;
        private readonly IHostingEnvironment _env;
        private readonly SignInManager<AdminApplicationUser> _signInManager;
        private readonly ILogger _logger;
        [TempData]
        public string ErrorMessage { get; set; }
        //user managers
        private readonly UserManager<AdminApplicationUser> _userManager;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly UserManager<TraineeApplicationUser> _traineeuserManager;

        public AdministratorController(ITrainerService trainer, UserManager<AdminApplicationUser> usermanager, SignInManager<AdminApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IHostingEnvironment env, UserManager<TraineeApplicationUser> traineeuserManager, ApplicationDbContext context, TraineeApplicationDbContext traineedbcontext, AdminApplicationDbContext _admindbcontext, ILogger<AccountController> logger)
        {
            _userManager = usermanager;
            _usermanager = userManager;
            _signInManager = signInManager;
            _logger = logger;
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
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Administrator", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToAction(nameof(Dashboard));
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var user = new AdminApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        SmtpClient client = new SmtpClient("smtp.office365.com"); //set client 
                        client.Port = 587;
                        client.EnableSsl = true;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential("Wragbydev@wragbysolutions.com", "@Devops19"); //Mailing credential
                                                                                                                  //mail body
                        MailMessage mailMessage = new MailMessage();
                        mailMessage.From = new MailAddress("Wragbydev@wragbysolutions.com");
                        mailMessage.To.Add(model.Email); //Trainee mail here
                        mailMessage.Body = "Hello, your mail was just used to log in on mistdo.azurewebsites.net, Did you initiate this ? if not you can reply this mail for Support";
                        mailMessage.Subject = "MISTDO Account Created";
                        client.Send(mailMessage);
                        return RedirectToAction(nameof(Dashboard));
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
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
            string id = null;
            var allModuletrainee = await _trainer.GetAllModuleTrainees();
            var Trainings = await dbcontext.Trainings.ToListAsync();
            foreach (var item in Trainings)
                ViewBag.centerid = item.TrainingCentreId;

            
            var modules = new List<Modules>();
            foreach (var item in allModuletrainee)
            {
                modules.Add(admindbcontext.Modules.FirstOrDefault(m => m.Id.ToString() == item.ModuleId));
            }
            double cos = 0;
            //var modules = await _trainer.GetAllModules();
                 ViewBag.modulecosts = modules;
            

            double total = modules.Select(i => i.Cost).Sum();
            string cost = string.Format(new System.Globalization.CultureInfo("en-NG"), "{0:C2}", total);
           
            ViewBag.totalcost = cost;

            return View(Trainings);

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
    
        public async Task<IActionResult> Logout()
            {
                //await _signInManager.SignOutAsync();
           
                return RedirectToAction(nameof(AdministratorController.Login), "Administrator");
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
           var trainer = await dbcontext.Users.SingleOrDefaultAsync(m => m.Id == calenders.TrainingCentreId);
            ViewBag.center = trainer.CentreName;
            if (calenders == null)
            {
                return NotFound();
            }

            return View(calenders);
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
        public async Task<IActionResult> AssignTrainerModule()
        {
            var modules = await _trainer.GetAllModules();

            var modulesList = new List<SelectListItem>();
            foreach (var item in modules)

                modulesList.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });


            ViewBag.modules = modulesList;

            

            ViewBag.Message = await dbcontext.TrainerModules.ToListAsync();

            var module = await admindbcontext.Modules.ToListAsync();
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTrainerModule(AssignTrainerModule assign, string trainer)
        {
            var user = await _usermanager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                var modulename = await _trainer.GetModulebyId(int.Parse(assign.ModuleId));
                var updatemodule = new AssignTrainerModule
                {
                    CentreId = trainer,
                    ModuleId = assign.ModuleId,
                    ModuleName = modulename.Name,
                    DateGenerated = DateTime.Now,
                  

                };

                dbcontext.Add(updatemodule);
                await dbcontext.SaveChangesAsync();
                return RedirectToAction(nameof(AssignTrainerModule));
            }
            else
            {
                return View(assign);
            }

           

         
        }

        public async Task<IActionResult> DeleteTrainerModule(int id)
        {
            var modules = await dbcontext.TrainerModules.SingleOrDefaultAsync(m => m.Id == id);
            if (modules == null)
            {
                return RedirectToAction(nameof(AssignTrainerModule));
            }

            try
            {
                dbcontext.TrainerModules.Remove(modules);
                await dbcontext.SaveChangesAsync();
                return RedirectToAction(nameof(AssignTrainerModule));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
               // return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }

           
            dbcontext.TrainerModules.Remove(modules);
           var result =   await admindbcontext.SaveChangesAsync();
            return RedirectToAction(nameof(AssignTrainerModule));

          
        }

     
        [AllowAnonymous]
        public async Task<IActionResult> DeActivate(string id)
        {
           
            var AppUser = await _usermanager.FindByIdAsync(id);
            AppUser.PhoneNumberConfirmed = true;
            var idResult = await _usermanager.UpdateAsync(AppUser);
            if (idResult.Succeeded) 
            {
                return RedirectToAction("AllTrainingCenter");
            }
            return RedirectToAction("AllTrainingCenter");
        }

  
        [AllowAnonymous]
        public async Task<IActionResult> Activate(string id)
        {

            var AppUser = await _usermanager.FindByIdAsync(id);
            AppUser.PhoneNumberConfirmed = false;
            var idResult = await _usermanager.UpdateAsync(AppUser);
            if (idResult.Succeeded)
            {
                return RedirectToAction("AllTrainingCenter");
            }
            return RedirectToAction("AllTrainingCenter");
        }

        // POST: Training/Delete/5
        //[HttpPost, ActionName("DeleteTrainerModule")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirm(int id)
        //{
        //    var modules = await dbcontext.TrainerModules.SingleOrDefaultAsync(m => m.Id == id);
        //    dbcontext.TrainerModules.Remove(modules);
        //    await admindbcontext.SaveChangesAsync();
        //    return RedirectToAction(nameof(AssignTrainerModule));
        //}

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Modules(Modules model,string trainer)
        {

            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(EligibleUsersForCertificate), new { ModuleId = model.Id, trainerid = trainer });
            }
            return View(model);
        }
        public async Task<IActionResult> EligibleUsersForCertificate(int ModuleId, string trainerid)
        {
            var users = new List<SelectListItem>();

            var us =  _usermanager.Users.FirstOrDefault(t => t.UID == trainerid);
             var Trainer = await _usermanager.FindByIdAsync(us.Id);
            

            //   var traineesList = new List<TraineeViewModel>();
            var trainings = await _trainer.GetNullCertificateTrainees(Trainer.Id, ModuleId.ToString());

            foreach (var trainee in trainings)
            {
                var TraineeApplicationUser = Traineedbcontext.Users.Where(t => t.UID == trainee.TraineeId).ToList();

                foreach (var user in TraineeApplicationUser)
                {
                    users.Add(new SelectListItem { Text = user.UserName, Value = user.Id });

                }

            }
            ViewBag.users = users;
            ViewBag.Trainer = Trainer.Id;

            ViewBag.ModuleId = ModuleId;

            return View();


        }


        [HttpPost]

        public IActionResult EligibleUsersForCertificate(NewCertificateViewModel model,string trainer)
        {
            if (model.TraineeId == null)
            {
                return Content("No Available Trainee");
            }
            if (ModelState.IsValid)
            {


                return RedirectToAction(nameof(Payment), new { traineeid = model.TraineeId, moduleid = model.ModuleId,trainerid = trainer });
            }
            return View(model);
        }
        public async Task<IActionResult> Payment(string traineeid, int moduleid,string trainerid)
        {
            var module = await _trainer.GetModulebyId(moduleid);
            var trainee = Traineedbcontext.Users.FirstOrDefault(t => t.Id == traineeid);
            var TraineeViewModel = new Models.AccountViewModels.TraineeViewModel
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
            ViewBag.Trainer = trainerid;
           // return RedirectToAction(nameof(ViewCertificate), new { traineeid = trainee.UID, moduleid = model.ModuleId, trainerid = trainer });
            return View(TraineeViewModel);
        }

        public IActionResult PaymentCompleted(/*RemitaResponse data,*/ string traineeid, string moduleid)
        {
            //var training = new Training
            //{
            //    PaymentRefId = data.PaymentReference,


            //};

            //  var trainee = _traineeuserManager.FindByIdAsync(traineeid);
            var url = Url.Action(nameof(ViewCertificate), new { traineeid, moduleid });
            return Json(new { isSuccess = true, redirectUrl = url });
            //  return RedirectToAction(nameof(ViewCertificate), new {  traineeid, moduleid });
        }

        public async Task<IActionResult> GenerateCertificate(string traineeid, string moduleid,string trainer)
        {
            var user = await _usermanager.FindByIdAsync(trainer);

            var trainee = await _traineeuserManager.FindByIdAsync(traineeid);
            var train = dbcontext.Trainings.ToList();
            var training = train.FirstOrDefault(t => t.TraineeId == trainee.UID & t.ModuleId == moduleid & t.TrainingCentreId == user.Id);

            var centre = await _usermanager.FindByIdAsync(user.Id);


            var module = admindbcontext.Modules.FirstOrDefault(m => m.Id == int.Parse(moduleid));

            if (training.CertificateId != null)
            {
                return Content("Cert already Generated");
            }

            var CertId = "MISTDO/" + module.ShortCode + "/" + DateTime.Now.Year + "/" + Helpers.GetCertId.RandomString(5);

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
            // Then Send Mail
            SmtpClient client = new SmtpClient("smtp.office365.com"); //set client 
            client.Port = 587;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("Wragbydev@wragbysolutions.com", "@Devops19"); //Mailing credential
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("Wragbydev@wragbysolutions.com");
            mailMessage.To.Add("Femi4god2010@gmail.com"); //swap with trainee.Email on go live
            mailMessage.Body = "Dear " + trainee.FirstName + ", Congratulations!! You have been Awarded a Certifcate on " + module.Name + " by " + user.CompanyName + " on " + DateTime.Now + " Go to your Training Provider to Collect your Certificate.";
            mailMessage.Subject = "MISTDO Certificate ";
            client.Send(mailMessage);


            ViewBag.Training = updateTraining;



            return View();
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
            foreach (var item in tcentre)
                ViewBag.CenterID = item.UID;

            var users = await Traineedbcontext.Users.ToListAsync();
            foreach (var item in users)
                ViewBag.trainee = item.UID;

          

            var training = await dbcontext.Trainings
                .SingleOrDefaultAsync(m => m.Id == id);

            var trainee = await Traineedbcontext.Users.SingleOrDefaultAsync(a => a.UID == training.TraineeId);
            ViewBag.traineeid = trainee.Id;
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

            var trainee = await Traineedbcontext.Users.SingleOrDefaultAsync(m => m.Id == id);
            var train = await dbcontext.Trainings.Where(t => t.TraineeId == trainee.UID).ToListAsync();

            ViewBag.trainings = train;



            if (id == null)
            {
                return View(await Traineedbcontext.Users.ToListAsync());
            }

           


            ViewBag.ID = trainee.UID;


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
            var trainee = await _traineeuserManager.FindByIdAsync(traineeid);
            var training = dbcontext.Trainings.FirstOrDefault(t => t.TraineeId == trainee.UID && t.ModuleId == moduleid && t.TrainingCentreId == TrainingCentreId);
            var centre = await _usermanager.FindByIdAsync(TrainingCentreId);

            
            var module = admindbcontext.Modules.FirstOrDefault(m => m.Id == int.Parse(moduleid));
            //QR code generation
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(training.CertificateId, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            //Bitmap qrCodeImage = qrCode.GetGraphic(20);
            //Set color by using Color-class types
            Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.Goldenrod, Color.White, true);
            MemoryStream ms = new MemoryStream();
            qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
           

            ViewBag.QR = ms.ToArray();

            ViewBag.Trainee = trainee;
            ViewBag.Centre = centre;
            ViewBag.Module = module;

            ViewBag.Training = training;



            return View();
        }

        public async Task<IActionResult> IdCards(string traineeid, string moduleid, string TrainingCentreId, int TrainingId)
        {
            var trainee = await _traineeuserManager.FindByIdAsync(traineeid);
            var training = dbcontext.Trainings.FirstOrDefault(t => t.TraineeId == trainee.UID && t.ModuleId == moduleid && t.TrainingCentreId == TrainingCentreId);
            var centre = await _usermanager.FindByIdAsync(TrainingCentreId);


            var module = admindbcontext.Modules.FirstOrDefault(m => m.Id == int.Parse(moduleid));
            //QR code generation
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(training.CertificateId, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            //Bitmap qrCodeImage = qrCode.GetGraphic(20);
            //Set color by using Color-class types
            Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.Goldenrod, Color.White, true);
            MemoryStream ms = new MemoryStream();
            qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);


            ViewBag.QR = ms.ToArray();

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
                  
                };

                admindbcontext.Add(updatemodule);
                await admindbcontext.SaveChangesAsync();
                return RedirectToAction(nameof(AllModules));
            }
            else
            {
                return View(modules);
            }
        }
        [HttpGet]
        [Route("ExportCenters")]
        public IActionResult ExportCenters()
        {
            string rootFolder = _env.WebRootPath;
            string fileName = @"ExportAllCenters.xlsx";
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
                worksheet.Cells[1, 6].Value = "Permit NUmber";

                int i = 0;
                for (int row = 2; row <= totalRows + 1; row++)
                {
                    worksheet.Cells[row, 1].Value = traineeList[i].Id;
                    worksheet.Cells[row, 2].Value = traineeList[i].CentreName;
                    worksheet.Cells[row, 3].Value = traineeList[i].Email;
                    worksheet.Cells[row, 4].Value = traineeList[i].CompanyAddress;
                    worksheet.Cells[row, 5].Value = traineeList[i].PhoneNumber;
                    worksheet.Cells[row, 6].Value = traineeList[i].PermitNumber;

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
            string fileName = @"ExportApprovedModules.xlsx";
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
                worksheet.Cells[1, 8].Value = "State";
                worksheet.Cells[1, 9].Value = "City";
                worksheet.Cells[1, 10].Value = "Registration Date";
                int i = 0;
                for (int row = 2; row <= totalRows + 1; row++)
                {
                    worksheet.Cells[row, 1].Value = traineeList[i].FirstName;
                    worksheet.Cells[row, 2].Value = traineeList[i].LastName;
                    worksheet.Cells[row, 3].Value = traineeList[i].UID;
                    worksheet.Cells[row, 4].Value = traineeList[i].Email;
                    worksheet.Cells[row, 5].Value = traineeList[i].CompanyName;
                    worksheet.Cells[row, 6].Value = traineeList[i].CompanyAddress;
                    worksheet.Cells[row, 7].Value = traineeList[i].UserAddress;
                    worksheet.Cells[row, 8].Value = traineeList[i].State.ToString();
                    worksheet.Cells[row, 9].Value = traineeList[i].City.ToString();
                    worksheet.Cells[row, 10].Value = traineeList[i].DateRegistered.ToString();
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
            string fileName = @"ExportRegisteredTraineesOnModules.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);

            FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(rootFolder, fileName));
            }

            using (ExcelPackage package = new ExcelPackage(file))
            {


                //

                //ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Trainings");
                //using (var cells = worksheet.Cells[1, 1, 1, 10]) //(1,1) (1,5)
                //{
                //    cells.Style.Font.Bold = true;
                //}
                //int totalRows = trainingList.Count();


                //worksheet.Cells[1, 1].Value = "Trainee ID";
                //worksheet.Cells[1, 2].Value = "Training Name";
                //worksheet.Cells[1, 3].Value = "Training Centre ID";
                //worksheet.Cells[1, 4].Value = "Certificate ID";
                //worksheet.Cells[1, 5].Value = "Date Registered";
                //worksheet.Cells[1, 6].Value = "Training Start Date";
                //worksheet.Cells[1, 7].Value = "Training End Date";
                //worksheet.Cells[1, 8].Value = "Certifcate Gen  Date";


                //int i = 0;
                //for (int row = 2; row <= totalRows + 1; row++)
                //{
                //    worksheet.Cells[row, 1].Value = trainingList[i].TraineeId;
                //    worksheet.Cells[row, 2].Value = trainingList[i].TrainingName;
                //    worksheet.Cells[row, 3].Value = trainingList[i].TrainingCentreId;
                //    worksheet.Cells[row, 4].Value = trainingList[i].CertificateId;
                //    worksheet.Cells[row, 5].Value = trainingList[i].DateCreated.ToString();
                //    worksheet.Cells[row, 6].Value = trainingList[i].TrainingStartDate.Date.ToString();
                //    worksheet.Cells[row, 7].Value = trainingList[i].TrainingEndDate.Date.ToString();
                //    worksheet.Cells[row, 8].Value = trainingList[i].CertGenDate.Date.ToString();
                //    i++;
                //}

                //package.Save();

                IList<Training> training = dbcontext.Trainings.ToList();
                string id = null;
                foreach (var item in training)
                    id = item.TraineeId;

                var user = await _usermanager.GetUserAsync(User);
                List<TraineeApplicationUser> Trainees = new List<TraineeApplicationUser>();

                var trainings = dbcontext.Trainings.ToList();

                foreach (var trainee in trainings)
                {
                    var tra = Traineedbcontext.Trainees.Where(t => t.UID == trainee.TraineeId);
                    Trainees.AddRange(tra);

                }

               // return View(Trainees.Distinct());
               // IList<TraineeApplicationUser> traineeList = Traineedbcontext.Users.Where(x=>x.UID == id ).ToList();

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("User");
                using (var cells = worksheet.Cells[1, 1, 1, 10]) //(1,1) (1,5)
                {
                    cells.Style.Font.Bold = true;
                }

                int totalRows = Trainees.Count();

                worksheet.Cells[1, 1].Value = "First Name";
                worksheet.Cells[1, 2].Value = "Last Name";
                worksheet.Cells[1, 3].Value = "Trainee ID";
                worksheet.Cells[1, 4].Value = "Email";
                worksheet.Cells[1, 5].Value = "Company Name";
                worksheet.Cells[1, 6].Value = "Company Address";
                worksheet.Cells[1, 7].Value = "User Address";
                worksheet.Cells[1, 8].Value = "State";
                worksheet.Cells[1, 9].Value = "City";
                worksheet.Cells[1, 10].Value = "Registration Date";
                int i = 0;
                for (int row = 2; row <= totalRows + 1; row++)
                {
                    worksheet.Cells[row, 1].Value = Trainees[i].FirstName;
                    worksheet.Cells[row, 2].Value = Trainees[i].LastName;
                    worksheet.Cells[row, 3].Value = Trainees[i].UID;
                    worksheet.Cells[row, 4].Value = Trainees[i].Email;
                    worksheet.Cells[row, 5].Value = Trainees[i].CompanyName;
                    worksheet.Cells[row, 6].Value = Trainees[i].CompanyAddress;
                    worksheet.Cells[row, 7].Value = Trainees[i].UserAddress;
                    worksheet.Cells[row, 8].Value = Trainees[i].State.ToString();
                    worksheet.Cells[row, 9].Value = Trainees[i].City.ToString();
                    worksheet.Cells[row, 10].Value = Trainees[i].DateRegistered.ToString();
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
