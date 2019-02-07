using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models
{
    public class OgispTemp
    {
        [Key]

        public int Id { get; set; }
        public string PermitNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string Email { get; set; }
        public string LicenseExpDate { get; set; }
        public string Otp { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime DateCreated { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public DateTime Time { get; set; }


    }
}
