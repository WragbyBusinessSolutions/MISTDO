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
        public int TrainingId { get; set; }
        public string TrainingName { get; set; }
        public int TrainingCost { get; set; }
        public DateTime TrainingStartDate{ get; set; }
        public DateTime TrainingEndDate { get; set; }
        public int CentreId { get; set; }


    }
}
