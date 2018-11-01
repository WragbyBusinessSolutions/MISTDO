using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Services
{
    public interface IExcelToTraineeService
    {
       Task ConvertFileToTraineeString(string filePath);
    }
}
