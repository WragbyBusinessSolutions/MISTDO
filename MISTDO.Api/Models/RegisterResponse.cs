using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Api.Models
{
    public class RegisterResponse
    {
        public string UserId { get; set; }
        public string token { get; set; }
        public string scheme { get; set; }

    }
}
