﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MISTDO.Web.Data;
using MISTDO.Web.Models;
using Microsoft.AspNetCore.Http;
using System.Threading;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Identity;
using MISTDO.Web.Services;
using Microsoft.Extensions.Logging;
using MISTDO.Web.Controllers;
using Microsoft.AspNetCore.Authentication;
using MISTDO.Web.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace MISTDO.Web.Views.TrainerDashboard
{
    [Authorize]
    public class TraineesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        public ApplicationDbContext dbcontext { get; }
        public IExcelToTraineeService _exceltoTrainee { get; }

        private readonly IHostingEnvironment _envt;

        public TraineesController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger, IHostingEnvironment env, IExcelToTraineeService excelToTraineeService, ApplicationDbContext context)
        {
            _context = context;
            dbcontext = context;
            _exceltoTrainee = excelToTraineeService;
            _envt = env;

            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;

        }

        // GET: Trainees
        public async Task<IActionResult> Index()
        {
            return View(await _context.Trainees.ToListAsync());
        }

        public async Task<IActionResult> Profile(string Email, TraineeRegisterViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
           
            


            if (Email == null)
            {
                return View(await _context.Trainees.ToListAsync());
            }

           if(user.Email == model.Email )
            { 
            var trainee = await _context.Trainees.SingleOrDefaultAsync(m => m.Email == Email);





            if (trainee == null)
            {
                return NotFound();
            }

            return View(trainee);

            }
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> Training(Training training)
        {
            return View(await _context.Training.ToListAsync());
        }
        // GET: Trainees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return View(await _context.Trainees.ToListAsync());
            }

            var trainee = await _context.Trainees.SingleOrDefaultAsync(m => m.TraineeId == id);




            if (trainee == null)
            {
                return NotFound();
            }

            return View(trainee);
        }

        public IActionResult ForgotPassword()
        {

            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterNow(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterNow(Trainee trainee, Microsoft.AspNetCore.Http.IFormFile ImageUpload, TraineeRegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                //copy a replical to the trainee table 
                _context.Add(trainee);
                await _context.SaveChangesAsync();
                var user = new ApplicationUser()
                {

                    UserName = model.Email,
                    Email = model.Email,

                    PhoneNumber = model.PhoneNo,
                    CompanyAddress = model.CompanyAddress,
                    CompanyName = model.CompanyName,
                    UserAddress = model.UserAddress,
                    FirstName = model.FirstName,
                    LastName = model.LastName,

                    State = model.State,
                    City = model.City,
                    Country = model.Country,
                    DateRegistered = DateTime.Now.Date,



                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Trainer");
                    var centredetails = new TrainingCentre
                    {
                        CentreName = model.CentreName,
                        OGISPUserName = model.OGISPUserName,
                        OGISPId = model.OGISPId,
                        User = user


                    };
                    dbcontext.Add(centredetails);
                    dbcontext.SaveChanges();


                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    var response = _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    
                   
                    return View("ConfirmMail");

                }


            }

            if (ModelState.IsValid)
            {
                //if (ImageUpload != null)

                //{
                //    if (ImageUpload.Length > 0)

                //    //Convert Image to byte and save to database

                //    {

                //        byte[] p1 = null;
                //        using (var fs1 = ImageUpload.OpenReadStream())
                //        using (var ms1 = new MemoryStream())
                //        {
                //            fs1.CopyTo(ms1);
                //            p1 = ms1.ToArray();
                //        }
                //        trainee.ImageUpload = p1;

                //    }
                //}



               
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(TraineeLoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var isTrainee = await _userManager.IsInRoleAsync(user, "Trainee");
                    //if (isTrainer)
                    //{
                    returnUrl = returnUrl ?? Url.Content("~/Trainees/Dashboard");
                    //     }
                    //      else
                    //      {
                    return RedirectToLocal(returnUrl);
                    //      }
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
        // GET: Trainees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Trainees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Trainee trainee, Microsoft.AspNetCore.Http.IFormFile ImageUpload)
        {

           

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
                        trainee.ImageUpload = p1;

                    }
                }



                _context.Add(trainee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trainee);
        }

        // GET: Trainees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainee = await _context.Trainees.SingleOrDefaultAsync(m => m.TraineeId == id);
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
        public async Task<IActionResult> Edit(int id,  Trainee trainee, Microsoft.AspNetCore.Http.IFormFile ImageUpload)
        {
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
                        trainee.ImageUpload = p1;

                    }
                }
                try
                {
                    

                    _context.Update(trainee);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            return View(trainee);
        }

        // GET: Trainees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainee = await _context.Trainees.SingleOrDefaultAsync(m => m.TraineeId == id);
            if (trainee == null)
            {
                return NotFound();
            }

            return View(trainee);
        }

        // POST: Trainees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainee = await _context.Trainees.SingleOrDefaultAsync(m => m.TraineeId == id);
            _context.Trainees.Remove(trainee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TraineeExists(int id)
        {
            return _context.Trainees.Any(e => e.TraineeId == id);
        }


        [HttpPost("UploadFile")]
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
            return RedirectToAction("Index");

        }

    }
}
