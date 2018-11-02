﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models
{
    public class Modules
    {
        [Key]


        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public double Cost { get; set; }
        public string ShortCode { get; set; }
        public double CertificateCost { get; set; }

    }
}
