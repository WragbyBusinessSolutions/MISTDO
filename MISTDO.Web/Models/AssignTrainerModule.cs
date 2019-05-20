using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models
{
    public class AssignTrainerModule
    {
        [Key]
        public int Id { get; set; }
        public string CentreId { get; set; }
        public string ModuleId { get; set; }
        public string ModuleName { get; set; }

        [DataType(DataType.Date)]

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]

        public DateTime DateGenerated { get; set; }


    }
}
