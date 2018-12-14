using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models
{
    public class Calender
    {
        [Key]
         
        public int Id { get; set; }
       
        public string ModuleId { get; set; }
       
        public string TrainingName { get; set; }

        public string Venue { get; set; }
        public decimal Cost { get; set; }

        public string TrainingCentreId { get; set; }

       
        [DataType(DataType.Date)]

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]

        public DateTime TrainingStartDate{ get; set; }

       
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]

        public DateTime TrainingEndDate { get; set; }

      
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public DateTime TrainingStartTime { get; set; }

       
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public DateTime TrainingEndTime { get; set; }

    }
}
