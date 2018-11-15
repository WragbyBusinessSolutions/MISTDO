using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MISTDO.Web.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class TraineeApplicationUser : IdentityUser
    {
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(100)]
        [Remote("doesUserNameExist", "Account", HttpMethod = "POST", ErrorMessage = "User name already exists. Please enter a different user name.")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]

        [MaxLength(100)]
        public string LastName { get; set; }
        [MaxLength(100)]
        public string CompanyAddress { get; set; }

        [MaxLength(100)]
        public string CompanyName { get; set; }

        [MaxLength(100)]
        public string UserAddress { get; set; }
        public DateTime DateRegistered { get; set; }

        public byte[] FirstFinger { get; set; }
        public byte[] MiddleFinger { get; set; }
        public byte[] LastFinger { get; set; }

        public byte[] ImageUpload { get; set; }

    }
}
