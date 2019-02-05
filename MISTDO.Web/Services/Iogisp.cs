using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MISTDO.Web.Models;
using MISTDO.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Services
{
   public  interface Iogisp
    {

        Task<OgispResponse> GetOgisp(string PermitNumber);
    }
}
