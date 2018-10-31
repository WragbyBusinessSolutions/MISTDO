using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MISTDO.Web.Data;
using MISTDO.Web.Models;
using MISTDO.Web.Services;

namespace MISTDO.Web.Views
{
    public class TrainingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        public IExcelToTrainingService _exceltoTraining { get; }

        public TrainingController(ApplicationDbContext context, IHostingEnvironment env, IExcelToTrainingService excelToTrainingService)
        {
            _context = context;
            _exceltoTraining = excelToTrainingService;
            _env = env;
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
                .SingleOrDefaultAsync(m => m.TrainingId == id);
            if (training == null)
            {
                return NotFound();
            }

            return View(training);
        }

        // GET: Training/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Training/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TrainingName,TrainingCost,TrainingStartDate,TrainingEndDate")] Training training)
        {
            if (ModelState.IsValid)
            {
                //var dex = Convert.ToInt32(trainingCentre.CentreId);
                //var bex = Convert.ToInt32(training.CentreId);
                //  bex = dex;


                _context.Add(training);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(training);
        }

        // GET: Training/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _context.Trainings.SingleOrDefaultAsync(m => m.TrainingId == id);
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
        public async Task<IActionResult> Edit(int id, [Bind("TrainingId,TrainingName,TrainingCost,CentreId,TrainingStartDate,TrainingEndDate")] Training training)
        {
            if (id != training.TrainingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(training);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingExists(training.TrainingId))
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
            if (id == null)
            {
                return NotFound();
            }

            var training = await _context.Trainings.SingleOrDefaultAsync(m => m.TrainingId == id);
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
            var training = await _context.Trainings.SingleOrDefaultAsync(m => m.TrainingId == id);
            _context.Trainings.Remove(training);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainingExists(int id)
        {
            return _context.Trainings.Any(e => e.TrainingId == id);
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
    }
}
