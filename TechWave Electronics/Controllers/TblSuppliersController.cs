using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechWave_Electronics.Models;

namespace TechWave_Electronics.Controllers
{
    public class TblSuppliersController : Controller
    {
        private readonly TechWaveElectronics _context;
        private readonly ICustomKeyManager _keyManager;

        public TblSuppliersController(TechWaveElectronics context, ICustomKeyManager keyManager)
        {
            _context = context;
            _keyManager = keyManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var suppliers = await _context.TblSuppliers.ToListAsync();

            var protectedSuppliers = suppliers.Select(s =>
            {
                s.Id = _keyManager.Protect(s.SupplierId.ToString());
                return s;
            });

            return View(protectedSuppliers);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                var supplierId = Guid.Parse(_keyManager.Unprotect(id));

                var supplier = await _context.TblSuppliers
                    .FirstOrDefaultAsync(s => s.SupplierId == supplierId);

                if (supplier == null) return NotFound();

                ViewData["EncryptedId"] = id;
                return View(supplier);
            }
            catch (CryptographicException)
            {
                return BadRequest("Invalid encrypted ID.");
            }
            catch (FormatException)
            {
                return BadRequest("ID format is invalid.");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SupplierId,ContactNumber,Name,Address,City,Province")] TblSupplier tblSupplier)
        {
            if (tblSupplier != null) 
            {
                tblSupplier.SupplierId = Guid.NewGuid();
                _context.Add(tblSupplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblSupplier);

        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                var supplierId = Guid.Parse(_keyManager.Unprotect(id));
                var supplier = await _context.TblSuppliers.FindAsync(supplierId);

                if (supplier == null) return NotFound();

                ViewData["EncryptedId"] = id;
                return View(supplier);
            }
            catch
            {
                return BadRequest("Invalid request.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("SupplierId,ContactNumber,Name,Address,City,Province")] TblSupplier tblSupplier)
        {
            if (!ModelState.IsValid) return View(tblSupplier);

            try
            {
                var decryptedId = Guid.Parse(_keyManager.Unprotect(id));

                if (tblSupplier.SupplierId != decryptedId)
                    return BadRequest("Mismatched ID.");

                _context.Update(tblSupplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblSupplierExists(tblSupplier.SupplierId))
                    return NotFound();

                throw;
            }
            catch
            {
                ModelState.AddModelError("", "Unable to save changes.");
            }

            ViewData["EncryptedId"] = id;
            return View(tblSupplier);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                var supplierId = Guid.Parse(_keyManager.Unprotect(id));

                var supplier = await _context.TblSuppliers
                    .FirstOrDefaultAsync(s => s.SupplierId == supplierId);

                if (supplier == null) return NotFound();

                ViewData["EncryptedId"] = id;
                return View(supplier);
            }
            catch
            {
                return BadRequest("Invalid ID.");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var decryptedId = Guid.Parse(_keyManager.Unprotect(id));
                var supplier = await _context.TblSuppliers.FindAsync(decryptedId);

                if (supplier != null)
                {
                    _context.TblSuppliers.Remove(supplier);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return BadRequest("Failed to delete supplier.");
            }
        }

        private bool TblSupplierExists(Guid id)
        {
            return _context.TblSuppliers.Any(e => e.SupplierId == id);
        }
    }
}
