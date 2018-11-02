using System;
using System.Collections.Generic;
using MISTDO.Web.Models;
namespace MISTDO.Web.ViewModels
{
    public class TraineeViewModel
    {
        public IEnumerable<TraineeApplicationUser> Exams { get; set; }
    }
}
