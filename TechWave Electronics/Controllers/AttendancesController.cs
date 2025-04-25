using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechWave_Electronics.Models;

namespace TechWave_Electronics.Controllers
{
    public class AttendancesController : Controller
    {
        private readonly TechWaveElectronics _context;
        private readonly ICustomKeyManager _keyManager;

        public AttendancesController(TechWaveElectronics context, ICustomKeyManager keyManager)
        {
            _context = context;
            _keyManager = keyManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var attendances = await _context.Attendance.Include(a => a.Employee).ToListAsync();

            var encryptedAttendances = attendances.Select(a =>
            {
                a.Id = _keyManager.Protect(a.AttendanceID.ToString());
                return a;
            });

            return View(encryptedAttendances);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            try
            {
                var decryptedId = _keyManager.Unprotect(id);

                if (!Guid.TryParse(decryptedId, out var attendanceId))
                    return BadRequest("Invalid ID format.");

                var attendance = await _context.Attendance
                    .Include(a => a.Employee)
                    .FirstOrDefaultAsync(a => a.AttendanceID == attendanceId);

                if (attendance == null)
                    return NotFound();

                ViewData["EncryptedId"] = _keyManager.Protect(attendance.AttendanceID.ToString());
                return View(attendance);
            }
            catch (CryptographicException)
            {
                return BadRequest("Invalid ID format.");
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
        public async Task<IActionResult> Create([Bind("AttendanceID,EmployeeId,AttendanceDate,CheckIn,CheckOut")] Attendance attendance)
        {
            if (!ModelState.IsValid)
            {
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", attendance.EmployeeId);
                return View(attendance);
            }

            attendance.AttendanceID = Guid.NewGuid();
            _context.Add(attendance);
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
                var decryptedId = Guid.Parse(_keyManager.Unprotect(id));
                var attendance = await _context.Attendance.FindAsync(decryptedId);

                if (attendance == null)
                    return NotFound();

                ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", attendance.EmployeeId);
                return View(attendance);
            }
            catch
            {
                return BadRequest("Invalid request.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("AttendanceID,EmployeeId,AttendanceDate,CheckIn,CheckOut")] Attendance attendance)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", attendance.EmployeeId);
                return View(attendance);
            }

            try
            {
                var decryptedId = Guid.Parse(_keyManager.Unprotect(id));

                if (decryptedId != attendance.AttendanceID)
                    return BadRequest("ID mismatch.");

                _context.Update(attendance);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttendanceExists(attendance.AttendanceID))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            try
            {
                var decryptedId = Guid.Parse(_keyManager.Unprotect(id));
                var attendance = await _context.Attendance
                    .Include(a => a.Employee)
                    .FirstOrDefaultAsync(a => a.AttendanceID == decryptedId);

                if (attendance == null)
                    return NotFound();

                return View(attendance);
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
            var decryptedId = Guid.Parse(_keyManager.Unprotect(id));
            var attendance = await _context.Attendance.FindAsync(decryptedId);

            if (attendance != null)
                _context.Attendance.Remove(attendance);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AttendanceExists(Guid id)
        {
            return _context.Attendance.Any(e => e.AttendanceID == id);
        }
    }
}
