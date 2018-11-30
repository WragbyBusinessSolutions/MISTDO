using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models.AdminViewModels
{
    public class TwoModel
    {
        public IEnumerable<TrainingCentre> Training { get; set; }
        public IEnumerable<Modules> Module { get; set; }
    }
}
