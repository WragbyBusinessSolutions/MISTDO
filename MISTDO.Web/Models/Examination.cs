using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models
{
    public class Examination
    {
        [Key]


        public int ExamId { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public string ShortCode { get; set; }


    }
}
