using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MISTDO.Web.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {

       

        [Required]
        [MaxLength(100)]
        public string CompanyName { get; set; }
        [Required]
        [MaxLength(100)]
        public string CompanyAddress { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(100)]
        public string CentreName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(100)]
        public string CentreAddress { get; set; }
     
        public string State { get; set; }
        public string City { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime DateRegistered { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(100)]
        public string Otp { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(100)]
        public string LicenseExpDate { get; set; }

        [Required]
        public string PermitNumber { get; set; }
        public string UID { get; set; }

        public byte[] ImageUpload { get; set; }

    }
}
