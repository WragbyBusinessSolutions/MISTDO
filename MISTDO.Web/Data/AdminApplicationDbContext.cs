using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MISTDO.Web.Models;

namespace MISTDO.Web.Data
{
    public class AdminApplicationDbContext : IdentityDbContext<AdminApplicationUser>
    {
        public AdminApplicationDbContext(DbContextOptions<AdminApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
           
        }

        public DbSet<Modules> Modules { get; set; }


    }
}
