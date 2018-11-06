using MISTDO.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;

namespace MISTDO.Web.Services
{
    public interface ITrainerService
    {
        Task<IEnumerable<Certificate>> GetAllCertificates();
        Task<IEnumerable<Training>> GetAllTrainees(string TrainingCentreId, string ModuleId);
        Task<IEnumerable<Training>> GetNullCertificateTrainees(string TrainingCentreId, string ModuleId);
        Task<IEnumerable<Training>> GetAllModuleTrainees();
        Task<IEnumerable<Training>> GetTraining(string Id);

        Task<IEnumerable<Training>> GetTrainee();
        Task<IEnumerable<Certificate>> GetCertificate(string Id);
        Task<IEnumerable<TraineeApplicationUser>> GetTrainees();
        Task<IEnumerable<Modules>> GetAllModules();
        Task<IEnumerable<Calender>> GetCalenders();
        Task<IEnumerable<ApplicationUser>> GetAllTrainingCenters();
        Task<Modules> GetModulebyId(int Id);

    }
}
