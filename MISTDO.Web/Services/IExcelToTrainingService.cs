using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Services
{
    public interface IExcelToTrainingService
    {
       Task ConvertFileToTrainingString(string filePath);
    }
}
