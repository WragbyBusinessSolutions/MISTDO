using MISTDO.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.ViewModels
{
    public class NewCertificateViewModel
    {
        public Certificate Certificate { get; set; }
        public int TrainerId { get; set; }
    }
}
