using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using TechWave_Electronics.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace TechWave_Electronics.Controllers
{
    [ActivityLog]
    //[Authorize(Roles = "HR Manager,Management,Administrators")]
    public class DepartmentsController : Controller
    {
        private readonly TechWaveElectronics _context;
        private readonly ICustomKeyManager _keyManager;


        public DepartmentsController(TechWaveElectronics context,ICustomKeyManager keyManager)
        {
            _context = context;
            _keyManager = keyManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var department = _context.Departments.ToList().Select(d =>
            {
                d.Id = _keyManager.Protect(d.DepartmentsId.ToString());
                return d;
            });
            return View(department);
        }

        [Authorize(Roles = "HR Manager,Management,Administrators")]
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            Guid depId;
            try
            {
                string unprotectedId = _keyManager.Unprotect(id);
                if (!Guid.TryParse(unprotectedId, out depId))
                {
                    return BadRequest("Invalid ID format.");
                }
            }
            catch (FormatException)
            {
                return BadRequest("Invalid ID format.");
            }
            catch (CryptographicException)
            {
                return BadRequest("Invalid ID format.");
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.DepartmentsId == depId);
            if (department == null)
            {
                return NotFound();
            }

            ViewData["EncryptedId"] = _keyManager.Protect(department.DepartmentsId.ToString());
            return View(department);
        }

        [Authorize(Roles = "HR Manager,Management,Administrators")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentsId,DepartmentName,Location")] Department department)
        {
            if (department != null)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        [Authorize(Roles = "HR Manager,Management,Administrators")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                Guid depId = Guid.Parse(_keyManager.Unprotect(id));
                var department = await _context.Departments.FindAsync(depId);
                if (department == null)
                {
                    return NotFound();
                }
                ViewData["EncryptedId"] = id;
                return View(department);
            }
            catch (Exception)
            {
                return BadRequest("Invalid request.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("DepartmentsId,DepartmentName,Location")] Department department)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            Guid decryptedId;
            try
            {
                decryptedId = Guid.Parse(_keyManager.Unprotect(id));
            }
            catch (Exception)
            {
                return BadRequest("Invalid request.");
            }

            if (department != null)
            {
                try
                {
                    var existingDepartment = await _context.Departments.FindAsync(decryptedId);
                    if (existingDepartment == null)
                    {
                        return NotFound();
                    }

                    existingDepartment.DepartmentName = department.DepartmentName;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.DepartmentsId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "The changes could not be saved. Try again, and if the problem persists, contact your system administrator.");
                }
            }

            ViewData["EncryptedId"] = id;
            return View(department);
        }

        [Authorize(Roles = "HR Manager,Management,Administrators")]
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Guid depId = Guid.Parse(_keyManager.Unprotect(id));

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.DepartmentsId == depId);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            Guid depId = Guid.Parse(_keyManager.Unprotect(id));

            var department = await _context.Departments.FindAsync(depId);
            if (department != null)
            {
                _context.Departments.Remove(department);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(Guid id)
        {
            return _context.Departments.Any(e => e.DepartmentsId == id);
        }
    }
}
