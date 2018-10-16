using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models.AccountViewModels
{
    public class RegisterTraineeViewModel
    {
        // Login Details

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

       


        [Display(Name = "First Name")]
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(100)]
        [Display(Name = "Company Address")]
        public string CompanyAddress { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(100)]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(100)]
        [Display(Name = "User Address")]
        public string UserAddress { get; set; }

                
       
        
        [Display(Name = "Date Registered")]

        public DateTime DateRegistered { get; set; }
        [Required]
        [Display(Name = "Phone Number")]

        
        public string PhoneNo{ get; set; }

        // [Required]
        // public byte[] FirstFinger { get; set; }
        // [Required]
        // public byte[] MiddleFinger { get; set; }
        // [Required]
        // public byte[] LastFinger { get; set; }

       
    }
}

