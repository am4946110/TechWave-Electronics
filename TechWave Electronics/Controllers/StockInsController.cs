using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechWave_Electronics.Models;

namespace TechWave_Electronics.Controllers
{
    public class StockInsController : Controller
    {
        private readonly TechWaveElectronics _context;
        private readonly ICustomKeyManager _keyManager;

        public StockInsController(TechWaveElectronics context, ICustomKeyManager keyManager)
        {
            _context = context;
            _keyManager = keyManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var stockIns = await _context.StockIn.ToListAsync();
            foreach (var stockIn in stockIns)
            {
                stockIn.Id = _keyManager.Protect(stockIn.StockInId.ToString());
            }
            return View(stockIns);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            Guid decryptedId;
            try
            {
                decryptedId = Guid.Parse(_keyManager.Unprotect(id));
            }
            catch (Exception)
            {
                return BadRequest("Invalid ID format.");
            }

            var stockIn = await _context.StockIn
                .FirstOrDefaultAsync(m => m.StockInId == decryptedId);
            if (stockIn == null)
                return NotFound();

            return View(stockIn);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,WarehouseId,SupplierId,Quantity,DateReceived")] StockIn stockIn)
        {
            if (ModelState.IsValid)
            {
                stockIn.StockInId = Guid.NewGuid();
                _context.Add(stockIn);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(stockIn);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            Guid decryptedId;
            try
            {
                decryptedId = Guid.Parse(_keyManager.Unprotect(id));
            }
            catch (Exception)
            {
                return BadRequest("Invalid ID format.");
            }

            var stockIn = await _context.StockIn.FindAsync(decryptedId);
            if (stockIn == null)
                return NotFound();

            return View(stockIn);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("StockInId,ItemId,WarehouseId,SupplierId,Quantity,DateReceived")] StockIn stockIn)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            Guid decryptedId;
            try
            {
                decryptedId = Guid.Parse(_keyManager.Unprotect(id));
            }
            catch (Exception)
            {
                return BadRequest("Invalid ID format.");
            }

            if (decryptedId != stockIn.StockInId)
                return BadRequest("Mismatched ID.");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingStockIn = await _context.StockIn.FindAsync(decryptedId);
                    if (existingStockIn == null)
                        return NotFound();

                    existingStockIn.ItemId = stockIn.ItemId;
                    existingStockIn.WarehouseId = stockIn.WarehouseId;
                    existingStockIn.SupplierId = stockIn.SupplierId;
                    existingStockIn.Quantity = stockIn.Quantity;
                    existingStockIn.DateReceived = stockIn.DateReceived;

                    _context.Update(existingStockIn);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockInExists(decryptedId))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(stockIn);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            Guid decryptedId;
            try
            {
                decryptedId = Guid.Parse(_keyManager.Unprotect(id));
            }
            catch (Exception)
            {
                return BadRequest("Invalid ID format.");
            }

            var stockIn = await _context.StockIn
                .FirstOrDefaultAsync(m => m.StockInId == decryptedId);
            if (stockIn == null)
                return NotFound();

            return View(stockIn);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            Guid decryptedId;
            try
            {
                decryptedId = Guid.Parse(_keyManager.Unprotect(id));
            }
            catch (Exception)
            {
                return BadRequest("Invalid ID format.");
            }

            var stockIn = await _context.StockIn.FindAsync(decryptedId);
            if (stockIn != null)
            {
                _context.StockIn.Remove(stockIn);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool StockInExists(Guid id)
        {
            return _context.StockIn.Any(e => e.StockInId == id);
        }
    }
}
