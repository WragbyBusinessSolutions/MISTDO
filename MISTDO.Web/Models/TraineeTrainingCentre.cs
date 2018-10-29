using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models
{
    public class TraineeTrainingCentre
    {
        [Key]
        public int CentreId { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(100)]
        public string CentreName { get; set; }

        [Required]
        [DataType(DataType.Text)]

        [MaxLength(100)]
        public string OGISPUserName { get; set; }

        [Required]

        public string OGISPId { get; set; }
        [Required]

        public virtual TraineeApplicationUser User { get; set; }



    }
}
