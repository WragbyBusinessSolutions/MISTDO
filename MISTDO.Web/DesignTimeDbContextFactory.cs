using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MISTDO.Web.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TraineeApplicationDbContext>
    {
        public TraineeApplicationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
            var builder = new DbContextOptionsBuilder<TraineeApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("TraineeConnection");
            builder.UseSqlServer(connectionString);
            return new TraineeApplicationDbContext(builder.Options);
        }
    }

    public class AdminDesignTimeDbContextFactory : IDesignTimeDbContextFactory<AdminApplicationDbContext>
    {
        public AdminApplicationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                                  .SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json")
                                  .Build();
            var builder = new DbContextOptionsBuilder<AdminApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("AdminConnection");
            builder.UseSqlServer(connectionString);
            return new AdminApplicationDbContext(builder.Options);
        }
    }

}
