﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MISTDO.Web.Data;
using MISTDO.Web.Models;
using MISTDO.Web.Services;
using MISTDO.Web.ViewModels;

namespace MISTDO.Web.Controllers
{
    [Authorize]
    public class TrainerDashboardController : Controller
    {
        public ITrainerService _trainer { get; }

        private readonly ApplicationDbContext dbcontext;

        public TrainerDashboardController(ITrainerService trainer, ApplicationDbContext context)
        {
            _trainer = trainer;
            dbcontext = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Certificate()
        {
            var certs = await _trainer.GetAllCertificates();
            return View(certs);
        }
        public IActionResult Trainee()
        {
            return View();
        }
        // GET: Certificates/Create
        public IActionResult NewCertificate()
        {

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewCertificate(NewCertificateViewModel model)
        {
            var tt = dbcontext.Trainees.FirstOrDefault(t => t.TraineeId == model.TrainerId); 
            model.Certificate.DateGenerated = DateTime.Now;
            model.Certificate.Owner = tt;
            if (ModelState.IsValid)
            {
                dbcontext.Add(model.Certificate);
                await dbcontext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}