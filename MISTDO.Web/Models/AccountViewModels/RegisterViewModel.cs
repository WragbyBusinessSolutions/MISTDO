using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        // Login Details

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

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
        [Display(Name = "Country")]
        [Required]
        [DataType(DataType.Text)]
        public string Country { get; set; }
        [Display(Name = "State")]
        [Required]
        [DataType(DataType.Text)]
        public string State { get; set; }
        [Display(Name = "City")]
        [Required]
        [DataType(DataType.Text)]
        public string City { get; set; }
        [Display(Name = "Date Registered")]

        public DateTime DateRegistered { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        public string OGISPId { get; set; }


        [Display(Name = "Training Centre Name")]
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(100)]
        public string CentreName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "OGISP UserName")]
        [MaxLength(100)]
        public string OGISPUserName { get; set; }
        [Required]

        public string TrainingModule { get; set; }
    }
}

