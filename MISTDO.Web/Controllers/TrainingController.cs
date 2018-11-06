using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MISTDO.Web.Data;
using MISTDO.Web.Models;
using MISTDO.Web.Services;

namespace MISTDO.Web.Views
{
    [Authorize]
    public class TrainingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _env;
        public IExcelToTrainingService _exceltoTraining { get; }

        public ITrainerService _trainer { get; }

        public TrainingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment env, ITrainerService trainer, IExcelToTrainingService excelToTrainingService)
        {
            _trainer = trainer;
            _context = context;
            _exceltoTraining = excelToTrainingService;
            _env = env;
            _userManager = userManager;
        }

        // GET: Training
        public async Task<IActionResult> Index()
        {
            return View(await _context.Trainings.ToListAsync());
        }

        // GET: Training/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _context.Trainings
                .SingleOrDefaultAsync(m => m.Id == id);
            if (training == null)
            {
                return NotFound();
            }

            return View(training);
        }

        // GET: Training/Create
        public async Task<IActionResult> Create()
        {
            var modules = await _trainer.GetAllModules();

            var modulesList = new List<SelectListItem>();
            foreach (var item in modules)

                modulesList.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            ViewBag.modules = modulesList;

            

            return View();
        }

        // POST: Training/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ModuleId,CertificateId,TraineeId,CertificateId ,TrainingName,TrainingCentreId ,PaymentRefId,DateCreated ,CertGenDate ,TrainingStartDate,TrainingEndDate")] Training training, string id)
        {

            
            
           

            var user = await _userManager.GetUserAsync(User);
            id = user.Id;
            if (ModelState.IsValid)
            {
                var train = new Training()
                {

                    TrainingCentreId =  user.Id,
                    CertificateId = training.CertificateId,
                    ModuleId =    training.ModuleId,
                    TrainingStartDate = training.TrainingStartDate,
                    TraineeId =   training.TraineeId,
                    PaymentRefId =training.PaymentRefId,
                    CertGenDate = training.CertGenDate,
                    DateCreated = training.DateCreated,
                    TrainingEndDate = training.TrainingEndDate,
                    TrainingName = training.TrainingName

                    

                };

                 _context.Add(train);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(training);
        }

        // GET: Training/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            var training = await _context.Trainings.SingleOrDefaultAsync(m => m.Id == id);
            if (training == null)
            {
                return NotFound();
            }
            return View(training);
        }

        // POST: Training/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ModuleId,CertificateId,TraineeId,CertificateId ,TrainingName,TrainingCentreId ,PaymentRefId,DateCreated ,CertGenDate ,TrainingStartDate,TrainingEndDate")] Training training)
        {
            var user = await _userManager.GetUserAsync(User);
           // id = user.Id;
            if (id != training.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var train = new Training()
                    {

                        TrainingCentreId = user.Id,
                        CertificateId = training.CertificateId,
                        ModuleId = training.ModuleId,
                        TrainingStartDate = training.TrainingStartDate,
                        TraineeId = training.TraineeId,
                        PaymentRefId = training.PaymentRefId,
                        CertGenDate = training.CertGenDate,
                        DateCreated = training.DateCreated,
                        TrainingEndDate = training.TrainingEndDate,
                        TrainingName = training.TrainingName



                    };
                    _context.Update(train);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingExists(training.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(training);
        }

        // GET: Training/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            var training = await _context.Trainings.SingleOrDefaultAsync(m => m.Id == id);
            if (training == null)
            {
                return NotFound();
            }

            return View(training);
        }

        // POST: Training/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var training = await _context.Trainings.SingleOrDefaultAsync(m => m.Id == id);
            _context.Trainings.Remove(training);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainingExists(int id)
        {
            return _context.Trainings.Any(e => e.Id == id);
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> TrainingUpload(Microsoft.AspNetCore.Http.IFormFile file)
        {


            if (file == null)
                return Content("Argument null");

            //var mimetype = MimeMapping.MimeTypes.GetMimeMapping(file.FileName);
            //if (mimetype != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            //{
            //    return Content("Invalid Content Type");

            //}
            var filePath = Path.Combine(_env.WebRootPath, ("productfiles\\" + file.Name));

            if (file.Length > 0)
            {
                if (System.IO.File.Exists(filePath))
                {

                    System.IO.File.Delete(filePath);
                }
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                await _exceltoTraining.ConvertFileToTrainingString(filePath);
            }
            return RedirectToAction("Index");

        }
        public ActionResult DownloadFile()
        {
            string path = AppDomain.CurrentDomain.DynamicDirectory + "wwwroot/templates/";
            byte[] fileBytes = System.IO.File.ReadAllBytes(path + "training.xlsx");
            string fileName = "training.xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}
