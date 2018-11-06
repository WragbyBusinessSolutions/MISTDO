using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models
{
    public class Notification
    {
        [Key]
        [DisplayName("Notification Id")]
        public int NotificationId { get; set; }

        [DisplayName("Notification Message")]
        public string NotificationMessage { get; set; }

        [DisplayName("Notification Time")]
        public DateTime NotificationDateTime { get; set; }

    }
}
