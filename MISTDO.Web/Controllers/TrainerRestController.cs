using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MISTDO.Web.Data;

namespace MISTDO.Web.Controllers
{
    [Route("api/trainers")]
    public class TrainerRestController : Controller
    {
        private  ApplicationDbContext dbcontext;
        public TrainerRestController(ApplicationDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }
        [Produces("application/json")]
        [HttpGet("search")]
        public async Task<IActionResult> Search()
        {
            try
            {
                string term = HttpContext.Request.Query["term"].ToString();
                var names = dbcontext.Users.Where(p => p.CompanyName.Contains(term)).Select(p => p.CompanyName).ToList();
                return Ok(names);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Produces("application/json")]
        [HttpGet("search")]
        public async Task<IActionResult> searchid()
        {
            try
            {
                string term = HttpContext.Request.Query["term"].ToString();
                var names = dbcontext.Users.Where(p => p.CompanyName.Contains(term)).Select(p => p.UID).ToList();
                return Ok(names);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}