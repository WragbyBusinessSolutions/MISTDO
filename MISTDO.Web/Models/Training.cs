using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models
{
    public class Training
    {
        [Key]
         
        public int Id { get; set; }
        public string TraineeId { get; set; }
        public string ModuleId { get; set; }
        public string CertificateId { get; set; }
        public string TrainingName { get; set; }

        public string TrainingCentreId { get; set; }
        public string PaymentRefId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]

        public DateTime DateCreated { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]

        public DateTime CertGenDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]

        public DateTime TrainingStartDate{ get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]

        public DateTime TrainingEndDate { get; set; }

         }
}
