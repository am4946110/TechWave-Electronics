using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechWave_Electronics.Models;

namespace TechWave_Electronics.Controllers
{
    public class HistoriesController : Controller
    {
        private readonly TechWaveElectronics _context;
        private readonly ICustomKeyManager _keyManager;

        public HistoriesController(TechWaveElectronics context, ICustomKeyManager keyManager)
        {
            _context = context;
            _keyManager = keyManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var histories = await _context.History.ToListAsync();
            foreach (var history in histories)
            {
                history.Id = _keyManager.Protect(history.TransactionId.ToString());
            }
            return View(histories);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            if (!TryDecryptId(id, out Guid transactionId))
                return BadRequest("Invalid ID format.");

            var history = await _context.History.FirstOrDefaultAsync(h => h.TransactionId == transactionId);
            if (history == null)
                return NotFound();

            ViewData["EncryptedId"] = id;
            return View(history);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,WarehouseId,QuantityChange,TransactionType,Date,Notes")] History history)
        {
            if (ModelState.IsValid)
            {
                history.TransactionId = Guid.NewGuid();
                _context.Add(history);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(history);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (!TryDecryptId(id, out Guid transactionId))
                return BadRequest("Invalid request.");

            var history = await _context.History.FindAsync(transactionId);
            if (history == null)
                return NotFound();

            ViewData["EncryptedId"] = id;
            return View(history);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("TransactionId,ItemId,WarehouseId,QuantityChange,TransactionType,Date,Notes")] History history)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            if (!TryDecryptId(id, out Guid transactionId))
                return BadRequest("Invalid request.");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingHistory = await _context.History.FindAsync(transactionId);
                    if (existingHistory == null)
                        return NotFound();

                    // تحديث القيم
                    existingHistory.ItemId = history.ItemId;
                    existingHistory.WarehouseId = history.WarehouseId;
                    existingHistory.QuantityChange = history.QuantityChange;
                    existingHistory.TransactionType = history.TransactionType;
                    existingHistory.Date = history.Date;
                    existingHistory.Notes = history.Notes;

                    _context.Update(existingHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HistoryExists(transactionId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["EncryptedId"] = id;
            return View(history);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            if (!TryDecryptId(id, out Guid transactionId))
                return BadRequest("Invalid ID format.");

            var history = await _context.History.FirstOrDefaultAsync(h => h.TransactionId == transactionId);
            if (history == null)
                return NotFound();

            ViewData["EncryptedId"] = id;
            return View(history);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!TryDecryptId(id, out Guid transactionId))
                return BadRequest("Invalid request.");

            var history = await _context.History.FindAsync(transactionId);
            if (history != null)
            {
                _context.History.Remove(history);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool HistoryExists(Guid id)
        {
            return _context.History.Any(e => e.TransactionId == id);
        }

        private bool TryDecryptId(string encryptedId, out Guid decryptedId)
        {
            decryptedId = Guid.Empty;
            try
            {
                string decryptedString = _keyManager.Unprotect(encryptedId);
                return Guid.TryParse(decryptedString, out decryptedId);
            }
            catch (CryptographicException)
            {
                return false;
            }
        }
    }
}
