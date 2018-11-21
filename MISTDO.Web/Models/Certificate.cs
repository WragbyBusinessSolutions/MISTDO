using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models
{
    public class Certificate
    {
        [Key]

        [Display(Name = "Certificate Id")]

        public int CertId { get; set; }
        [Display(Name = "Certificate Number")]

        public string CertNumber { get; set; }
        [Display(Name = "Status")]

        public string CertStatus { get; set; }
        [Display(Name = "Trainer Organization Id")]

        public string TrainerOrgId { get; set; }
        [Display(Name = "Trainer Organization")]

        public string TrainerOrg { get; set; }

        [DataType(DataType.Date)]

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime DateGenerated { get; set; }
        public string  Owner { get; set; }
        public virtual ApplicationUser Trainer { get; set; }
        public int TrainingId { get; set; }

        public int  ModuleId { get; set; }

    }
}
