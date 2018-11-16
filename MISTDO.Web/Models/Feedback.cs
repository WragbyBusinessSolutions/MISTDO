using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models
{
    public class Feedback
    {

        [Key]

        public int Id { get; set; }
        public string FeedbackSubject { get; set; }
        public string FeedbackMessage { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime FeedbackTimeStamp { get; set; }


    }
}
