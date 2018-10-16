using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MISTDO.Web.Controllers
{
    [Authorize]
    public class TrainerDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}