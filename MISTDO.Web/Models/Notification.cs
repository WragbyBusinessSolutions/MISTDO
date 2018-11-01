﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public string NotificationMessage { get; set; }
        public DateTime NotificationDateTime { get; set; }

    }
}
