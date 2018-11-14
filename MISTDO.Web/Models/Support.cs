using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models
{
    public class Support
    {
        [Key]
        public int SupportId { get; set; }
        public string Subject { get; set; }
        public string Issue { get; set; }
        public string Response { get; set; }
        public DateTime SupportTimeStamp{ get; set; }

        public DateTime ResponseTimeStamp { get; set; }

    }
}
