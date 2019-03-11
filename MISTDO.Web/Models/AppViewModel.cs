using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace MISTDO.Web.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class AppViewModel
    {


        public IFormFile ImageUpload { get; set; }



    }
}
