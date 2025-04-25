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
    public class StockOutsController : Controller
    {
        private readonly TechWaveElectronics _context;
        private readonly ICustomKeyManager _keyManager;

        public StockOutsController(TechWaveElectronics context, ICustomKeyManager keyManager)
        {
            _context = context;
            _keyManager = keyManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var StockOut = _context.StockOut.ToList().Select(s =>
            {
                s.Id = _keyManager.Protect(s.StockOutId.ToString());
                return s;
            });
            return View(StockOut);
        }
        [HttpGet]
        public async Task<IActionResult> Details(string encryptedId)
        {
            if (string.IsNullOrWhiteSpace(encryptedId))
                return BadRequest("Invalid request.");

            try
            {
                var decryptedId = Guid.Parse(_keyManager.Unprotect(encryptedId));
                var stockOut = await _context.StockOut.FirstOrDefaultAsync(s => s.StockOutId == decryptedId);

                if (stockOut == null)
                    return NotFound();
                ViewData["EncryptedId"] = _keyManager.Protect(stockOut.StockOutId.ToString());

                return View(stockOut);
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid request.");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,WarehouseId,Quantity,DateIssued")] StockOut stockOut)
        {
            if (!ModelState.IsValid)
                return View(stockOut);

            stockOut.StockOutId = Guid.NewGuid();
            _context.Add(stockOut);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string encryptedId)
        {
            if (string.IsNullOrWhiteSpace(encryptedId))
                return BadRequest("Invalid request.");

            try
            {
                var decryptedId = Guid.Parse(_keyManager.Unprotect(encryptedId));
                var stockOut = await _context.StockOut.FindAsync(decryptedId);

                if (stockOut == null)
                    return NotFound();

                ViewData["EncryptedId"] = encryptedId;
                return View(stockOut);
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid request.");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string encryptedId, [Bind("StockOutId,ItemId,WarehouseId,Quantity,DateIssued")] StockOut stockOut)
        {
            if (string.IsNullOrWhiteSpace(encryptedId))
                return BadRequest("Invalid request.");

            Guid decryptedId;
            try
            {
                decryptedId = Guid.Parse(_keyManager.Unprotect(encryptedId));
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid request.");
            }

            if (decryptedId != stockOut.StockOutId)
                return BadRequest("Mismatched StockOut ID.");

            if (!ModelState.IsValid)
                return View(stockOut);

            try
            {
                var existingStockOut = await _context.StockOut.FindAsync(decryptedId);
                if (existingStockOut == null)
                    return NotFound();

                existingStockOut.ItemId = stockOut.ItemId;
                existingStockOut.WarehouseId = stockOut.WarehouseId;
                existingStockOut.Quantity = stockOut.Quantity;
                existingStockOut.DateIssued = stockOut.DateIssued;

                _context.Update(existingStockOut);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!StockOutExists(decryptedId))
                    return NotFound();

                throw;
            }
        }


        // GET: StockOuts/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var decryptedId = Guid.Parse(_keyManager.Unprotect(id));

            var stockOut = await _context.StockOut
                .FirstOrDefaultAsync(m => m.StockOutId == decryptedId);
            if (stockOut == null)
            {
                return NotFound();
            }

            return View(stockOut);
        }

        // POST: StockOuts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var decryptedId = Guid.Parse(_keyManager.Unprotect(id));

            var stockOut = await _context.StockOut.FindAsync(decryptedId);
            if (stockOut != null)
            {
                _context.StockOut.Remove(stockOut);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockOutExists(Guid id)
        {
            return _context.StockOut.Any(e => e.StockOutId == id);
        }
    }
}
