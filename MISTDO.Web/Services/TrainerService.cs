using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MISTDO.Web.Data;
using MISTDO.Web.Models;

namespace MISTDO.Web.Services
{
    public class TrainerService : ITrainerService
    {
        private readonly ApplicationDbContext dbcontext;
        private readonly TraineeApplicationDbContext _Traineedbcontext;
        private readonly AdminApplicationDbContext Admindbcontext;
        private readonly UserManager<TraineeApplicationUser> _userManager;

        public TrainerService(ApplicationDbContext context, UserManager<TraineeApplicationUser> userManager, TraineeApplicationDbContext traineecontext, AdminApplicationDbContext admindb)
        {
            dbcontext = context;
            _Traineedbcontext = traineecontext;
            Admindbcontext = admindb;
            _userManager = userManager;

        }
        public async Task<IEnumerable<Certificate>> GetAllCertificates()
        {
            var certs = await dbcontext.Certificates.Include(t=>t.Owner).Include(i => i.Trainer).ToListAsync();
            return certs;
        }

        public async Task<IEnumerable<Training>> GetTraining(string Id)
        {
            var training = await dbcontext.Trainings.Where(t => t.TraineeId == Id).ToListAsync();
            return training;
        }

        public async Task<IEnumerable<Calender>> GetCalenders()
        {
            var calenders = await dbcontext.Calenders.ToListAsync();
            return calenders;
        }

        public async Task<IEnumerable<Training>> GetTrainee()
        {
           
            var training = await dbcontext.Trainings.ToListAsync();
            
         
            return training;
        }



        public async Task<IEnumerable<Certificate>> GetCertificate(string Id)
        {
            var certificate = await dbcontext.Certificates.Where(t => t.Owner.Id == Id).ToListAsync();
            return certificate;
        }

        public async Task<IEnumerable<TraineeApplicationUser>> GetTrainees()
        {
            var allRegisteredTrainees = await _Traineedbcontext.Trainees.ToListAsync();

            return allRegisteredTrainees;
        }

        public async Task<IEnumerable<Training>> GetAllTrainees(string TrainingCentreId, string ModuleId)
        {
            var Trainings = await dbcontext.Trainings.Where(t => t.TrainingCentreId == TrainingCentreId && t.ModuleId == ModuleId).ToListAsync();
            return Trainings;
        }

        public async Task<IEnumerable<Training>> GetAllModuleTrainees()
        {
            var Trainings = await dbcontext.Trainings.ToListAsync();
            return Trainings;
        }

        public async Task<IEnumerable<Training>> GetNullCertificateTrainees(string TrainingCentreId, string ModuleId)
        {
            var Trainings = await dbcontext.Trainings.Where(t => t.TrainingCentreId == TrainingCentreId && t.ModuleId == ModuleId && t.CertificateId == null).ToListAsync();


            return Trainings;

        }

        public async Task<IEnumerable<Modules>> GetAllModules()
        {
            var modules = await Admindbcontext.Modules.ToListAsync();
            return modules;

        }
        public async Task<Modules> GetModulebyId(int Id)
        {
            var modules = await Admindbcontext.Modules.FirstOrDefaultAsync(i => i.Id == Id);
            return modules;

        }

        public async Task<IEnumerable<ApplicationUser>> GetAllTrainingCenters()
        {
            var users = await dbcontext.Users.ToListAsync();

            return users;
        }
        //public async Task<IEnumerable<ApplicationUser>> GetTrainingCentre(string id)
        //{

        //    var trainer = await dbcontext.Users.FirstOrDefaultAsync(i => i.Id == id);



        //    return trainer;
        //}
    }
}
