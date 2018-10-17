using MISTDO.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.ViewModels
{
    public class PaymentViewModel
    {
        public Trainee Trainee { get; set; }
        public IEnumerable<Examination> Exams { get; set; }
    }
}
