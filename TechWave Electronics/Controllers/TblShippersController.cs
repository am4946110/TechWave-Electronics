using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechWave_Electronics.Models;

namespace TechWave_Electronics.Controllers
{
    public class TblShippersController : Controller
    {
        private readonly TechWaveElectronics _context;
        private readonly ICustomKeyManager _keyManager;

        public TblShippersController(TechWaveElectronics context, ICustomKeyManager keyManager)
        {
            _context = context;
            _keyManager = keyManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var shippers = await _context.TblShippers.ToListAsync();

            var protectedShippers = shippers.Select(s =>
            {
                s.Id = _keyManager.Protect(s.ShipperId.ToString());
                return s;
            }).ToList();

            return View(protectedShippers);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                var decryptedId = Guid.Parse(_keyManager.Unprotect(id));
                var shipper = await _context.TblShippers.FirstOrDefaultAsync(s => s.ShipperId == decryptedId);

                if (shipper == null) return NotFound();

                ViewData["EncryptedId"] = id;
                return View(shipper);
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
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShipperId,CompanyName")] TblShipper tblShipper)
        {
            if (ModelState.IsValid)
            {
                tblShipper.ShipperId = Guid.NewGuid();
                _context.Add(tblShipper);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblShipper);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                var decryptedId = Guid.Parse(_keyManager.Unprotect(id));
                var shipper = await _context.TblShippers.FindAsync(decryptedId);

                if (shipper == null) return NotFound();

                return View(shipper);
            }
            catch
            {
                return BadRequest("Invalid request.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ShipperId,CompanyName")] TblShipper tblShipper)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                var decryptedId = Guid.Parse(_keyManager.Unprotect(id));

                if (decryptedId != tblShipper.ShipperId)
                    return BadRequest("ID mismatch between encrypted ID and submitted form.");

                var existing = await _context.TblShippers.FindAsync(decryptedId);
                if (existing == null) return NotFound();

                if (ModelState.IsValid)
                {
                    existing.CompanyName = tblShipper.CompanyName;

                    try
                    {
                        _context.Update(existing);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TblShipperExists(decryptedId))
                            return NotFound();
                        throw;
                    }
                }

                return View(tblShipper);
            }
            catch
            {
                return BadRequest("Invalid ID format.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                var decryptedId = Guid.Parse(_keyManager.Unprotect(id));
                var shipper = await _context.TblShippers.FirstOrDefaultAsync(s => s.ShipperId == decryptedId);

                if (shipper == null) return NotFound();

                return View(shipper);
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
                var shipper = await _context.TblShippers.FindAsync(decryptedId);

                if (shipper != null)
                {
                    _context.TblShippers.Remove(shipper);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return BadRequest("Invalid delete request.");
            }
        }

        private bool TblShipperExists(Guid id)
        {
            return _context.TblShippers.Any(e => e.ShipperId == id);
        }
    }
}
