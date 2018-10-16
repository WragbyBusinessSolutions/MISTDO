using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models
{
    public class Trainee
    {
        [Key]
        public int TraineeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }

        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string UserAddress { get; set; }
        public byte[] FirstFinger { get; set; }
        public byte[] MiddleFinger { get; set; }
        public byte[] LastFinger { get; set; }

        public virtual Certificate Cert { get; set; }
    }
}
