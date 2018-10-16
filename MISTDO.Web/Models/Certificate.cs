using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models
{
    public class Certificate
    {
        [Key]
        public int CertId { get; set; }
        public string CertNumber { get; set; }
        public string CertStatus { get; set; }
        public string TrainerOrgId { get; set; }
        public string TrainerOrg { get; set; }
        public DateTime DateGenerated { get; set; }


    }
}
