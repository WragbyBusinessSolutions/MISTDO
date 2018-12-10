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
            List<Models.Calender> productsFromExcel = new List<Models.Calender>();
            var ws = ep.Workbook.Worksheets["Sheet1"];

            for (int rw = 2; rw <= ws.Dimension.End.Row; rw++)
            {
                Models.Calender productFromExcel = new Models.Calender();
                if (ws.Cells[rw, 1].Value != null)
                {
                    //Get value from exel file cells by column and populate the Table
                    productFromExcel.TrainingName = ws.Cells[rw, 1].Value.ToString();
                    productFromExcel.TrainingCentreId = ws.Cells[rw, 2].Value.ToString();
                    productFromExcel.TraineeId = ws.Cells[rw, 2].Value.ToString();
                    productFromExcel.ModuleId = ws.Cells[rw, 4].Value.ToString();
                    productFromExcel.Cost = decimal.Parse(ws.Cells[rw, 5].Value.ToString());
                    productFromExcel.Venue = ws.Cells[rw, 6].Value.ToString();
                    productFromExcel.TrainingStartTime = DateTime.Parse(ws.Cells[rw, 7].Value.ToString());
                    productFromExcel.TrainingEndTime = DateTime.Parse(ws.Cells[rw, 8].Value.ToString());
                    productFromExcel.TrainingStartDate = DateTime.Parse(ws.Cells[rw, 9].Value.ToString());
                    productFromExcel.TrainingEndDate = DateTime.Parse(ws.Cells[rw, 10].Value.ToString());

                }
               
                    productsFromExcel.Add(productFromExcel); //add data to model
                    _context.Calenders.Add(productFromExcel); //add data to table db
                    await _context.SaveChangesAsync(); //sync database

            }



        


    }

}
}
