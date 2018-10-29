using MISTDO.Web.Data;
using MISTDO.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Globalization;

namespace MISTDO.Web.Services
{
    public class ExcelToTraineeService : IExcelToTraineeService
    {
        private readonly ApplicationDbContext _context;
        public ExcelToTraineeService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task ConvertFileToTraineeString(string filePath)
        {

            var eps = new ExcelPackage(new FileInfo(filePath));
            List<Models.Trainee> traineesFromExcel = new List<Models.Trainee>();
            var ws = eps.Workbook.Worksheets["Sheet1"];
            
            for (int rw = 2; rw <= ws.Dimension.End.Row; rw++)
            {
                Models.Trainee traineeFromExcel = new Models.Trainee();
                if (ws.Cells[rw, 1].Value != null)
                {

                    traineeFromExcel.FirstName = ws.Cells[rw, 1].Value.ToString();
                    traineeFromExcel.LastName = ws.Cells[rw, 2].Value.ToString();
                    traineeFromExcel.Email = ws.Cells[rw, 3].Value.ToString();
                    traineeFromExcel.PhoneNo = ws.Cells[rw, 4].Value.ToString();
                    traineeFromExcel.CompanyName = ws.Cells[rw, 5].Value.ToString();
                    traineeFromExcel.CompanyAddress= ws.Cells[rw, 6].Value.ToString();
                    traineeFromExcel.UserAddress = ws.Cells[rw, 7].Value.ToString();
                    //productFromExcel.TrainingCost = int.Parse(ws.Cells[rw, 2].Value.ToString());
                    //productFromExcel.TrainingStartDate = DateTime.Parse(ws.Cells[rw, 3].Value.ToString());
                    //productFromExcel.TrainingEndDate= DateTime.Parse(ws.Cells[rw, 4].Value.ToString());
                    
                    // productFromExcel.category = ws.Cells[rw, 7].Value.ToString();
                }
                //ProductContext productContext = new ProductContext();
                //productContext.Products.Add(productFromExcel);
                //productContext.SaveChanges();
                traineesFromExcel.Add(traineeFromExcel);
                _context.Trainees.Add(traineeFromExcel);
                _context.SaveChanges();
               

            }
                   

            
        }

    }
}
