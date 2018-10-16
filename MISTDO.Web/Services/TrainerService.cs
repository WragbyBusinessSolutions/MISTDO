using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MISTDO.Web.Data;
using MISTDO.Web.Models;

namespace MISTDO.Web.Services
{
    public class TrainerService : ITrainerService
    {
        private readonly ApplicationDbContext dbcontext;

        public TrainerService(ApplicationDbContext context)
        {
            dbcontext = context;
        }
        public async Task<IEnumerable<Certificate>> GetAllCertificates()
        {
            var certs = await dbcontext.Certificates.ToListAsync();
            return certs;
        }

        public async Task<IEnumerable<Trainee>> GetAllTrainees()
        {
            var train = await dbcontext.Trainees.ToListAsync();
            return train;

        }
    }
}
