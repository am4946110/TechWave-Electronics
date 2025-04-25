using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechWave_Electronics.Models;

namespace TechWave_Electronics.Controllers
{
    public class LeavesController : Controller
    {
        private readonly TechWaveElectronics _context;
        private readonly ICustomKeyManager _keyManager;


        public LeavesController(TechWaveElectronics context, ICustomKeyManager keyManager)
        {
            _context = context;
            _keyManager = keyManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var techWaveElectronics = _context.leaves.Include(l => l.Employee)
                                                      .ToList()
                                                      .Select(l=>
                                                      {
                                                          l.Id = _keyManager.Protect(l.LeaveID.ToString());
                                                          return l;
                                                      });
            return View(techWaveElectronics);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                Guid leav = Guid.Parse(_keyManager.Unprotect(id));
                var leave = await _context.leaves
                    .Include(l => l.Employee)
                    .FirstOrDefaultAsync(m => m.LeaveID == leav);
                if (leave == null)
                {
                    return NotFound();
                }

                ViewData["EncryptedId"] = _keyManager.Protect(leave.LeaveID.ToString());

                return View(leave);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LeaveID,EmployeeId,LeaveType,StartDate,EndDate,Status")] Leave leave)
        {
            if (ModelState.IsValid)
            {
                leave.LeaveID = Guid.NewGuid();
                _context.Add(leave);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", leave.EmployeeId);
            return View(leave);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                Guid leve = Guid.Parse(_keyManager.Unprotect(id));
                var leave = await _context.leaves.FindAsync(id);
                if (leave == null)
                {
                    return NotFound();
                }
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", leave.EmployeeId);
                return View(leave);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("LeaveID,EmployeeId,LeaveType,StartDate,EndDate,Status")] Leave leave)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", leave.EmployeeId);
                return View(leave);
            }

            try
            {
                Guid decryptedId = Guid.Parse(_keyManager.Unprotect(id));
                var existingLeave = await _context.leaves.FindAsync(decryptedId);

                if (existingLeave == null)
                    return NotFound();

                existingLeave.EmployeeId = leave.EmployeeId;
                existingLeave.LeaveType = leave.LeaveType;
                existingLeave.StartDate = leave.StartDate;
                existingLeave.EndDate = leave.EndDate;
                existingLeave.Status = leave.Status;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaveExists(leave.LeaveID))
                    return NotFound();

                throw;
            }
            catch
            {
                return BadRequest("Invalid request.");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                Guid employeeId = Guid.Parse(_keyManager.Unprotect(id));

                var leave = await _context.leaves
                    .Include(l => l.Employee)
                    .FirstOrDefaultAsync(m => m.LeaveID == employeeId);
                if (leave == null)
                {
                    return NotFound();
                }

                return View(leave);
            }
            catch (Exception)
            {
                return BadRequest("Invalid request.");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            Guid employeeId = Guid.Parse(_keyManager.Unprotect(id));

            var leave = await _context.leaves.FindAsync(employeeId);
            if (leave != null)
            {
                _context.leaves.Remove(leave);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeaveExists(Guid id)
        {
            return _context.leaves.Any(e => e.LeaveID == id);
        }
    }
}
