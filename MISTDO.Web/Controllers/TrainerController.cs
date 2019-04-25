using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MISTDO.Web.Data;
using MISTDO.Web.Models;
using MISTDO.Web.Models.AccountViewModels;
using MISTDO.Web.Services;

namespace MISTDO.Web.Controllers
{
    [Authorize/*(AuthenticationSchemes = "TrainerAuth")*/]
    public class TrainerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        SmtpClient SmtpServer;

        private readonly Iogisp _ogisp;
        // private readonly ILogger _logger;
        public ApplicationDbContext dbcontext { get; }
        

        public TrainerController( Iogisp ogisp,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
             ApplicationDbContext context)
        {
            dbcontext = context;
            _ogisp = ogisp;
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
        public async Task<IActionResult> Profile(string id)
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
                    //user.UserAddress = model.UserAddress;
                    //user.FirstName = model.FirstName;
                    //user.LastName = model.LastName;

                    user.State = model.State;
                    user.City = model.City;
                    user.PermitNumber = model.PermitNumber;
                    user.CentreName = model.CentreName;
                    user.LicenseExpDate = model.LicenseExpDate;
                    user.Otp = model.PermitNumber; //NB
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
        public async Task<IActionResult>  RegisterNow()
        {
           
            return View();

         
        }

        //[HttpPost]
       [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterNow(RegisterViewModel model,string Companyname ,string  Companyaddress, string CentreName, string CenterAddress, string Permitnumber,string email, string Licenseexpdate, string otp, string PhoneNumber,string State,string City,string Password,string ConfirmPassword,string fee, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var center = await _userManager.FindByEmailAsync(email);

                if (center == null)
                {
                    //Generate OTP

                    string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    string small_alphabets = "abcdefghijklmnopqrstuvwxyz";
                    string numbers = "1234567890";

                    string characters = numbers;

                    characters += alphabets + small_alphabets + numbers;

                    int length = 7;
                    string otps = string.Empty;
                    for (int i = 0; i < length; i++)
                    {
                        string character = string.Empty;
                        do
                        {
                            int index = new Random().Next(0, characters.Length);
                            character = characters.ToCharArray()[index].ToString();
                        } while (otps.IndexOf(character) != -1);
                        otps += character;
                    }
                    string permitotp = otps.ToUpper();

                    var user = new ApplicationUser()
                    {

                        UserName = email,
                        Email = email,

                        PhoneNumber = PhoneNumber,
                        CompanyAddress = Companyaddress,
                        CompanyName = Companyname,
                        //UserAddress = model.UserAddress,
                        //FirstName = model.FirstName,
                        //LastName = model.LastName,
                        //temp
                        Otp = otp,

                        State = State,
                        City = City,
                        CentreName = CentreName,
                        CentreAddress = CenterAddress,
                        PermitNumber = Permitnumber,
                        LicenseExpDate = Licenseexpdate,
                        DateRegistered = DateTime.Now.Date,
                        UID = "MISTDO/" + "HSE/" + permitotp,

                    };

                    var result = await _userManager.CreateAsync(user, Password);
                    if (result.Succeeded)
                    {

                        var og = await dbcontext.OgispTemps.SingleOrDefaultAsync(m => m.Otp == otp);
                        dbcontext.OgispTemps.Remove(og);
                        if (og !=null)
                        {
                            await dbcontext.SaveChangesAsync();
                        }


                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                        var response = _emailSender.SendEmailConfirmationAsync(email, callbackUrl);


                //        return View("ConfirmMail");
                        var url = Url.Action(nameof(Login));
                        return  Json(new { isSuccess = true, redirectUrl = url });

                    }

                   
                }
               


            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> OgispCheck(OgispResponse data)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

          
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> IDCheck(String PermitNumber)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            //Verify OGISP Permit Number
            var verifyID = await _ogisp.GetOgisp(PermitNumber);

            if(verifyID != null && verifyID.PermitNumber == PermitNumber )//If Success
            {
                //Generate OTP

                string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                string small_alphabets = "abcdefghijklmnopqrstuvwxyz";
                string numbers = "1234567890";

                string characters = numbers;
              
               characters += alphabets + small_alphabets + numbers;
              
                int length = 10;
                string otps = string.Empty;
                for (int i = 0; i < length; i++)
                {
                    string character = string.Empty;
                    do
                    {
                        int index = new Random().Next(0, characters.Length);
                        character = characters.ToCharArray()[index].ToString();
                    } while (otps.IndexOf(character) != -1);
                    otps += character;
                }
                string permitotp = otps.ToUpper();


                
                var ogisp = await dbcontext.OgispTemps.Where(t => t.Email == verifyID.ComppanyEmail).ToListAsync();// pulls the schema or instance of the db with corresponding otp
                var centre = ogisp.FirstOrDefault(t => t.Email == verifyID.ComppanyEmail);// pulls the table with corresponding otp
                if (centre != null && centre.Email == verifyID.ComppanyEmail)
                {

                    dbcontext.OgispTemps.Remove(centre);
                    if (ogisp != null)
                    {
                        await dbcontext.SaveChangesAsync();
                    }


                }
                //Save to Temp db
                var centredetails = new OgispTemp
                {
                    PermitNumber = verifyID.PermitNumber,
                    CompanyName = verifyID.CompanyName,
                    CompanyAddress = verifyID.CompanyAddress,
                    Email = verifyID.ComppanyEmail,
                    LicenseExpDate = verifyID.expiryDate,
                    Otp = permitotp,
                    DateCreated = DateTime.Now,
                    Time = DateTime.Now.ToLocalTime()

                };
                dbcontext.Add(centredetails);
                dbcontext.SaveChanges();

                // Then Send Mail
                SmtpClient client = new SmtpClient("smtp.office365.com"); //set client 
                client.Port = 587;
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("Wragbydev@wragbysolutions.com", "@Devops19"); //Mailing credential
                //mail body
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("Wragbydev@wragbysolutions.com");
                mailMessage.To.Add("Femi4god2010@gmail.com"); //swap with verifyID.ComppanyEmail on go live
                mailMessage.Body = "Your OTP is: " + permitotp;
                mailMessage.Subject = "OGISP AUTHENTICATION";
                client.Send(mailMessage);


                return View("OgispOtp");
            }

            return View("OgispCheck");
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> OgispRenew(OgispResponse data)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);


            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PermitRenew(String PermitNumber)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            //Verify OGISP Permit Number
            var verifyID = await _ogisp.GetOgisp(PermitNumber);

            if (verifyID != null && verifyID.PermitNumber == PermitNumber )//If Success
            {
                //&& DateTime.Parse(verifyID.expiryDate) >= DateTime.Now
                //Generate OTP

                //string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                //string small_alphabets = "abcdefghijklmnopqrstuvwxyz";
                //string numbers = "1234567890";

                //string characters = numbers;

                //characters += alphabets + small_alphabets + numbers;

                //int length = 10;
                //string otps = string.Empty;
                //for (int i = 0; i < length; i++)
                //{
                //    string character = string.Empty;
                //    do
                //    {
                //        int index = new Random().Next(0, characters.Length);
                //        character = characters.ToCharArray()[index].ToString();
                //    } while (otps.IndexOf(character) != -1);
                //    otps += character;
                //}
                //string permitotp = otps.ToUpper();



                //var ogisp = await dbcontext.OgispTemps.Where(t => t.Email == verifyID.ComppanyEmail).ToListAsync();// pulls the schema or instance of the db with corresponding otp
                //var centre = ogisp.FirstOrDefault(t => t.Email == verifyID.ComppanyEmail);// pulls the table with corresponding otp
                //if (centre != null && centre.Email == verifyID.ComppanyEmail)
                //{

                //    dbcontext.OgispTemps.Remove(centre);
                //    if (ogisp != null)
                //    {
                //        await dbcontext.SaveChangesAsync();
                //    }


                //}
                //Save to Temp db
                //var centredetails = new OgispTemp
                //{
                //    PermitNumber = verifyID.PermitNumber,
                //    CompanyName = verifyID.CompanyName,
                //    CompanyAddress = verifyID.CompanyAddress,
                //    Email = verifyID.ComppanyEmail,
                //    LicenseExpDate = verifyID.expiryDate,
                //    Otp = permitotp,
                //    DateCreated = DateTime.Now,
                //    Time = DateTime.Now.ToLocalTime()

                //};
                //dbcontext.Add(centredetails);
                //dbcontext.SaveChanges();

                //Update User (Training center) Information
                var center = await _userManager.FindByEmailAsync(verifyID.ComppanyEmail);

                if (center != null)
                {


                    //Otp = center.Otp,
                    center.PermitNumber = verifyID.PermitNumber;
                    center.LicenseExpDate = verifyID.expiryDate;
                    center.DateRegistered = DateTime.Now.Date;
                  

                    var result = await _userManager.UpdateAsync(center);
                    if (result.Succeeded)
                    {

                        var og = await dbcontext.OgispTemps.SingleOrDefaultAsync(m => m.Otp == center.Otp);
                        
                        if (og != null)
                        {
                            dbcontext.OgispTemps.Remove(og);
                            await dbcontext.SaveChangesAsync();
                        }


                        return View("Login");
                    }


                }


                // Then Send Mail
                //SmtpClient client = new SmtpClient("smtp.office365.com"); //set client 
                //client.Port = 587;
                //client.EnableSsl = true;
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //client.UseDefaultCredentials = false;
                //client.Credentials = new NetworkCredential("Wragbydev@wragbysolutions.com", "@Devops19"); //Mailing credential
                ////mail body
                //MailMessage mailMessage = new MailMessage();
                //mailMessage.From = new MailAddress("Wragbydev@wragbysolutions.com");
                //mailMessage.To.Add("Femi4god2010@gmail.com"); //swap with verifyID.ComppanyEmail on go live
                //mailMessage.Body = "Your OTP is: " + permitotp;
                //mailMessage.Subject = "OGISP AUTHENTICATION";
                //client.Send(mailMessage);


                //return View("OgispOtp");
            }

            return View("OgispRenew");
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> OgispOtp()
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> OtpCheck(String otp)
        {
            var ogisp = await dbcontext.OgispTemps.Where(t => t.Otp == otp).ToListAsync();
            var centre = ogisp.FirstOrDefault(t => t.Otp == otp);
            if (centre != null && centre.Otp == otp)
            {

                ViewBag.permit = centre.PermitNumber;
                ViewBag.email = centre.Email;
                ViewBag.companyname = centre.CompanyName;
                ViewBag.companyaddress = centre.CompanyAddress;
                ViewBag.expdate = centre.LicenseExpDate;
                ViewBag.otp = otp;

                return View(nameof(RegisterNow));

            }


            return View(nameof(OgispOtp));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmMail(OgispResponse data)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);


            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(OgispResponse data)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);


            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewBag.msg = "";
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
                    //Verify OGISP Permit Number
                    var verifyID = await _ogisp.GetOgisp(user.PermitNumber);

                    if (verifyID != null && verifyID.PermitNumber == user.PermitNumber )//If Success
                    {

                        if (user.PhoneNumberConfirmed == false)
                        {
                            var isTrainer = await _userManager.IsInRoleAsync(user, "Trainer");
                            //if (isTrainer)
                            //{
                            returnUrl = returnUrl ?? Url.Content("~/TrainerDashboard/");
                            //     }
                            //      else
                            //      {
                            ViewBag.msg = "Success";
                            return RedirectToLocal(returnUrl);
                        }
                        else
                        {
                            ViewBag.msg = "You have been De-Listed From Accessing Training Centers Portal Contact Admin";
                            return RedirectToAction(nameof(Login));
                        }
                        //      }

                    }
                    return RedirectToAction(nameof(OgispRenew));
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
            return RedirectToAction(nameof(TrainerController.Login), "Trainer");
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
    }
}