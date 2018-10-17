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
        Task<IEnumerable<Trainee>> GetAllTrainees();
        Task<IEnumerable<Examination>> GetAllExams();

    }
}
