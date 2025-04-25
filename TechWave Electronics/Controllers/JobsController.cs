using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechWave_Electronics.Models;

namespace TechWave_Electronics.Controllers
{
    public class JobsController : Controller
    {
        private readonly TechWaveElectronics _context;
        private readonly ICustomKeyManager _keyManager;

        public JobsController(TechWaveElectronics context, ICustomKeyManager keyManager)
        {
            _context = context;
            _keyManager = keyManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var jobs = await _context.Job.ToListAsync();

            var encryptedJobs = jobs.Select(j =>
            {
                j.Id = _keyManager.Protect(j.JobiD.ToString());
                return j;
            });

            return View(encryptedJobs);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            try
            {
                Guid jobId = Guid.Parse(_keyManager.Unprotect(id));

                var job = await _context.Job.FirstOrDefaultAsync(j => j.JobiD == jobId);
                if (job == null)
                    return NotFound();

                ViewData["EncryptedId"] = _keyManager.Protect(job.JobiD.ToString());
                return View(job);
            }
            catch
            {
                return BadRequest("Invalid ID format.");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JobiD,jobName,BaseSalary")] Job job)
        {
            if (!ModelState.IsValid)
                return View(job);

            job.JobiD = Guid.NewGuid();
            _context.Add(job);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            try
            {
                Guid jobId = Guid.Parse(_keyManager.Unprotect(id));
                var job = await _context.Job.FindAsync(jobId);

                if (job == null)
                    return NotFound();

                return View(job);
            }
            catch
            {
                return BadRequest("Invalid request.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("JobiD,jobName,BaseSalary")] Job job)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            if (!ModelState.IsValid)
                return View(job);

            try
            {
                Guid jobId = Guid.Parse(_keyManager.Unprotect(id));
                var existingJob = await _context.Job.FindAsync(jobId);

                if (existingJob == null)
                    return NotFound();

                existingJob.jobName = job.jobName;
                existingJob.BaseSalary = job.BaseSalary;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobExists(job.JobiD))
                    return NotFound();
                else
                    throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            try
            {
                Guid jobId = Guid.Parse(_keyManager.Unprotect(id));
                var job = await _context.Job.FirstOrDefaultAsync(j => j.JobiD == jobId);

                if (job == null)
                    return NotFound();

                return View(job);
            }
            catch
            {
                return BadRequest("Invalid request.");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                Guid jobId = Guid.Parse(_keyManager.Unprotect(id));
                var job = await _context.Job.FindAsync(jobId);

                if (job != null)
                {
                    _context.Job.Remove(job);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return BadRequest("Invalid request.");
            }
        }

        private bool JobExists(Guid id)
        {
            return _context.Job.Any(j => j.JobiD == id);
        }
    }
}
