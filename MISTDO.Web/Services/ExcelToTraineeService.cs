using MISTDO.Web.Data;
using MISTDO.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Globalization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;

namespace MISTDO.Web.Services
{
    public class ExcelToTraineeService : IExcelToTraineeService
    {
        private readonly ApplicationDbContext _context;
        private readonly TraineeApplicationDbContext dbcontext;
        private readonly UserManager<TraineeApplicationUser> _userManager;
      
        public ExcelToTraineeService(ApplicationDbContext context, UserManager<TraineeApplicationUser> userManager, TraineeApplicationDbContext tcontext)
        {
            _context = context;
            dbcontext = tcontext;
            _userManager = userManager;
        }
     

        public async Task ConvertFileToTraineeString(string filePath)
        {

            var eps = new ExcelPackage(new FileInfo(filePath));
            List<Models.TraineeApplicationUser> traineesFromExcel = new List<Models.TraineeApplicationUser>();
            var ws = eps.Workbook.Worksheets["Sheet1"];
            
            for (int rw = 2; rw <= ws.Dimension.End.Row; rw++)
            {
                Models.TraineeApplicationUser traineeFromExcel = new Models.TraineeApplicationUser();
                     
                if (ws.Cells[rw, 1].Value != null )
                {

                    
                    traineeFromExcel.FirstName = ws.Cells[rw, 1].Value.ToString();
                    traineeFromExcel.LastName = ws.Cells[rw, 2].Value.ToString();
                    traineeFromExcel.UserName = ws.Cells[rw, 3].Value.ToString();

                    ////hash password before upload

                    //string password = ws.Cells[rw, 3].Value.ToString();

                    //// generate a 128-bit salt using a secure PRNG
                    //byte[] salt = new byte[128 / 8];
                    //using (var rng = RandomNumberGenerator.Create())
                    //{
                    //    rng.GetBytes(salt);
                    //}

                    //// derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
                    //string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    //    password: password,
                    //    salt: salt,
                    //    prf: KeyDerivationPrf.HMACSHA1,
                    //    iterationCount: 10000,
                    //    numBytesRequested: 256 / 8));

                    string originalPassword = "Qwerty415#";

                    var result = await _userManager.AddPasswordAsync(traineeFromExcel, originalPassword);

                   



                    traineeFromExcel.Email = ws.Cells[rw, 4].Value.ToString();
                    traineeFromExcel.NormalizedEmail = ws.Cells[rw, 4].Value.ToString().ToUpper();
                    traineeFromExcel.NormalizedUserName = ws.Cells[rw, 4].Value.ToString().ToUpper();
                    
                    traineeFromExcel.DateRegistered = DateTime.Now;
                    traineeFromExcel.PhoneNumber = ws.Cells[rw, 5].Value.ToString();
                    traineeFromExcel.CompanyName = ws.Cells[rw, 6].Value.ToString();
                    traineeFromExcel.CompanyAddress= ws.Cells[rw, 7].Value.ToString();
                    traineeFromExcel.UserAddress = ws.Cells[rw, 8].Value.ToString();
                    
                }
                
                traineesFromExcel.Add(traineeFromExcel);
                dbcontext.Users.Add(traineeFromExcel);
                await dbcontext.SaveChangesAsync();
               

            }
                   

            
        }

    }
}
