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
    public class TraineeApplicationDbContext : IdentityDbContext<TraineeApplicationUser>
    {
        public TraineeApplicationDbContext(DbContextOptions<TraineeApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

        }
        public DbSet<MISTDO.Web.Models.Training> Trainings { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Support> TraineeSupports { get; set; }
        public DbSet<TraineeTrainingCentre> TraineeTrainingCentres { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<TraineeApplicationUser> Trainees { get; set; }


    }
}
