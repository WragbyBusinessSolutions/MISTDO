using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MISTDO.Api.Data;
using MISTDO.Api.Models;
using MISTDO.Api.ViewModels;

namespace MISTDO.Api.Controllers
{
    [Produces("application/json")]
    public class AccountController : Controller
    {
        //  private readonly ApplicationDbContext dbcontext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountController(/*ApplicationDbContext context,*/ UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            //     dbcontext = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [Route("api/register")]

        [HttpPost]
        public async Task<JsonResult> Register([FromBody] RegisterViewModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                return null;

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // authentication successful
                var response = new RegisterResponse
                {
                    scheme = Request.Scheme,
                    token = code,
                    UserId = user.Id
                };
                return Json(response);

            }
            else
            {
                return Json(null);
            }
        }
        [Route("api/login")]

        [HttpPost]
        public async Task<JsonResult> Login([FromBody]LoginViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                return Json(result);
            }
            catch (Exception)
            {
                return Json("user not found");
            }

        }
    }
}
