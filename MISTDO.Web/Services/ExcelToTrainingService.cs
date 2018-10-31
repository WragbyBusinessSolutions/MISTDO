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
    public class ExcelToTrainingService : IExcelToTrainingService
    {
        private readonly ApplicationDbContext _context;
        public ExcelToTrainingService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task ConvertFileToTrainingString(string filePath)
        {

            var ep = new ExcelPackage(new FileInfo(filePath));
            List<Models.Training> productsFromExcel = new List<Models.Training>();
            var ws = ep.Workbook.Worksheets["Sheet1"];
            
            for (int rw = 2; rw <= ws.Dimension.End.Row; rw++)
            {
                Models.Training productFromExcel = new Models.Training();
                if (ws.Cells[rw, 1].Value != null)
                {
                  
                    productFromExcel.TrainingName = ws.Cells[rw, 1].Value.ToString();
                    productFromExcel.TrainingCost = int.Parse(ws.Cells[rw, 2].Value.ToString());
                    productFromExcel.TrainingStartDate = DateTime.Parse(ws.Cells[rw, 3].Value.ToString());
                    productFromExcel.TrainingEndDate= DateTime.Parse(ws.Cells[rw, 4].Value.ToString());
                    productFromExcel.CentreId = int.Parse(ws.Cells[rw, 5].Value.ToString());
                    // productFromExcel.category = ws.Cells[rw, 7].Value.ToString();
                }
                //ProductContext productContext = new ProductContext();
                //productContext.Products.Add(productFromExcel);
                //productContext.SaveChanges();
                productsFromExcel.Add(productFromExcel);
                _context.Trainings.Add(productFromExcel);
                _context.SaveChanges();

            }
                   

            
        }

    }
}
