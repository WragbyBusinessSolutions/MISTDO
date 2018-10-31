using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MISTDO.Web.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class TraineeApplicationUser : IdentityUser
    {
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(100)]
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
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public DateTime DateRegistered { get; set; }



        public int TraineeId { get; set; }

    }
}
