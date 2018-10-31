using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MISTDO.Web.Data;
using MISTDO.Web.Models;
using MISTDO.Web.Models.AccountViewModels;
using MISTDO.Web.Services;

namespace MISTDO.Web.Controllers
{

    public class TrainerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
       // private readonly ILogger _logger;
        public ApplicationDbContext dbcontext { get; }
        

        public TrainerController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
             ApplicationDbContext context)
        {
            dbcontext = context;
         
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Profile(string id, Trainee model)
        {
            var user = await _userManager.GetUserAsync(User);
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
            var user = await _userManager.GetUserAsync(User);
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
            var user = await _userManager.GetUserAsync(User);
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

                    var idResult = await _userManager.UpdateAsync(user);//update
                }
                return RedirectToAction(nameof(Profile));
            }
            return View(model);

        }

        private bool TrainerExists(string id)
        {
            return dbcontext.Users.Any(e => e.Id == id);
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
        public async Task<IActionResult> RegisterNow(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {

                    UserName = model.Email,
                    Email = model.Email,

                    PhoneNumber = model.PhoneNumber,
                    CompanyAddress = model.CompanyAddress,
                    CompanyName = model.CompanyName,
                    UserAddress = model.UserAddress,
                    FirstName = model.FirstName,
                    LastName = model.LastName,

                    State = model.State,
                    City = model.City,
                    Country = model.Country,
                    CentreName = model.CentreName,
                    OGISPUserName = model.OGISPUserName,
                    OGISPId = model.OGISPId,
                    DateRegistered = DateTime.Now.Date,



                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    
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

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
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
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var isTrainer = await _userManager.IsInRoleAsync(user, "Trainer");
                    //if (isTrainer)
                    //{
                    returnUrl = returnUrl ?? Url.Content("~/TrainerDashboard/");
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
                  //  _logger.LogWarning("User account locked out.");
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
               // _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
              //  _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
               // _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
          //  _logger.LogInformation("User logged out.");
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
    }
}