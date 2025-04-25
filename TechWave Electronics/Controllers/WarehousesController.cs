using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechWave_Electronics.Models;

namespace TechWave_Electronics.Controllers
{
    public class WarehousesController : Controller
    {
        private readonly TechWaveElectronics _context;
        private readonly ICustomKeyManager _keyManager;


        public WarehousesController(TechWaveElectronics context, ICustomKeyManager keyManager)
        {
            _context = context;
            _keyManager = keyManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var Warehouses = _context.Warehouses
                .ToList()
                .Select(w => 
                {
                    w.Id = _keyManager.Protect(w.WarehouseId.ToString());
                    return w;
                });

            return View(Warehouses);
        }

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

            var warehouses = await _context.Warehouses
                .FirstOrDefaultAsync(m => m.WarehouseId == depId);
            if (warehouses == null)
            {
                return NotFound();
            }

            ViewData["EncryptedId"] = _keyManager.Protect(warehouses.WarehouseId.ToString());

            return View(warehouses);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WarehouseId,WarehouseName,Location")] Warehouses warehouses)
        {
            if (ModelState.IsValid)
            {
                warehouses.WarehouseId = Guid.NewGuid();
                _context.Add(warehouses);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(warehouses);
        }

        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                Guid depId = Guid.Parse(_keyManager.Unprotect(id));

                var warehouses = await _context.Warehouses.FindAsync(depId);
                if (warehouses == null)
                {
                    return NotFound();
                }

                ViewData["EncryptedId"] = id;
                return View(warehouses);
            }
            catch (Exception)
            {

                return BadRequest("Invalid request.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string encryptedId, [Bind("WarehouseId,WarehouseName,Location")] Warehouses warehouse)
        {
            if (string.IsNullOrWhiteSpace(encryptedId))
                return BadRequest("Invalid request.");

            Guid warehouseId;
            try
            {
                warehouseId = Guid.Parse(_keyManager.Unprotect(encryptedId));
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid request.");
            }

            if (warehouseId != warehouse.WarehouseId)
                return BadRequest("Mismatched warehouse ID.");

            if (!ModelState.IsValid)
                return View(warehouse);

            try
            {
                var existingWarehouse = await _context.Warehouses.FindAsync(warehouseId);
                if (existingWarehouse == null)
                    return NotFound();

                existingWarehouse.WarehouseName = warehouse.WarehouseName;
                existingWarehouse.Location = warehouse.Location;

                _context.Update(existingWarehouse);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {

                if (!WarehouseExists(warehouseId))
                    return NotFound();

                throw;
            }
        }

        private bool WarehouseExists(Guid id)
        {
            return _context.Warehouses.Any(e => e.WarehouseId == id);
        }

        // GET: Warehouses/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Guid depId = Guid.Parse(_keyManager.Unprotect(id));

            var warehouses = await _context.Warehouses
                .FirstOrDefaultAsync(m => m.WarehouseId == depId);
            if (warehouses == null)
            {
                return NotFound();
            }

            return View(warehouses);
        }

        // POST: Warehouses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            Guid depId = Guid.Parse(_keyManager.Unprotect(id));

            var warehouses = await _context.Warehouses.FindAsync(depId);
            if (warehouses != null)
            {
                _context.Warehouses.Remove(warehouses);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
