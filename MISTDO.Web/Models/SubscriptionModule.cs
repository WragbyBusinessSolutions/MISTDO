using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models
{
    public class SubscriptionModule
    {
        [Key]
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public int ModuleDescription { get; set; }

    }
}
