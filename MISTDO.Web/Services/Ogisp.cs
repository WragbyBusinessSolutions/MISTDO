using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MISTDO.Web.Models;
using MISTDO.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using MISTDO.Web.Helpers;

namespace MISTDO.Web.Services
{
   public  class  Ogisp: Iogisp
    {
        public string apiEndpointUrl = "https://elps.dpr.gov.ng/api/lookup";
        private HttpClient Client { get; }
        public Ogisp()
        {
            Client = new HttpClient();
            Client.Timeout = TimeSpan.FromSeconds(1020);
            Client.DefaultRequestHeaders.Add("Api-Key", Config.secret);

        }

        public async Task<OgispResponse> GetOgisp(string PermitNumber)
        {
            var codehash = GetHash.SHA512Hash(Config.PublicKey,Config.secret,PermitNumber.ToLower());
            var url = apiEndpointUrl + "/" + Config.PublicKey + "/" + codehash.ToUpper() + "?licenseno=" + PermitNumber.ToLower() ;

            try
            {
                using (var response = await Client.GetAsync(url))
                {
                    var strResult = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<OgispResponse>(strResult);

                    }
                    return null;
                }
            }
            catch (System.Exception e)
            {
                return null;
            }
        }


    }
}
