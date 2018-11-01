using MISTDO.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Services
{
    public interface ITrainerService
    {
        Task<IEnumerable<Certificate>> GetAllCertificates();
        Task<IEnumerable<Training>> GetAllTrainees(string TrainingCentreId, string ModuleId);
        Task<IEnumerable<Training>> GetNullCertificateTrainees(string TrainingCentreId, string ModuleId);

        
    }
}
