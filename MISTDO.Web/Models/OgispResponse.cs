using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Models
{
    public class OgispResponse
    {

        [JsonProperty("number")]
        public string PermitNumber { get; set; }
        [JsonProperty("companyName")]

        public string CompanyName { get; set; }
        [JsonProperty("companyAddress")]

        public string CompanyAddress { get; set; }
        [JsonProperty("email")]

        public string ComppanyEmail { get; set; }
        [JsonProperty("expiryDate")]

        public string expiryDate { get; set; }

    }

    public class FailResponse
    {
        [JsonProperty("code")]

        public string Code { get; set; }
        [JsonProperty("message")]

        public string Message { get; set; }
      
    }
    public class NullResponse
    {
        [JsonProperty("code")]

        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
      

      
    }

}
