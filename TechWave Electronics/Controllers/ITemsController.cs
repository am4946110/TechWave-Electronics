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
    public class ITemsController : Controller
    {
        private readonly TechWaveElectronics _context;
        private readonly ICustomKeyManager _keyManager;

        public ITemsController(TechWaveElectronics context, ICustomKeyManager keyManager)
        {
            _context = context;
            _keyManager = keyManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var techWaveElectronics = _context.Items
                .Include(i => i.Category).ToList()
                .Select(i => 
                {
                    i.Id = _keyManager.Protect(i.ItemId.ToString());
                    return i;
                });
            return View(techWaveElectronics);
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

            var iTems = await _context.Items
                .Include(i => i.Category)
                .FirstOrDefaultAsync(m => m.ItemId == depId);
            if (iTems == null)
            {
                return NotFound();
            }

            ViewData["EncryptedId"] = _keyManager.Protect(iTems.ItemId.ToString());

            return View(iTems);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categorys, "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,ItemName,Description,CategoryID,UnitPrice,ReoderLevel")] ITems iTems)
        {
            if (ModelState.IsValid)
            {
                iTems.ItemId = Guid.NewGuid();
                _context.Add(iTems);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Categorys, "CategoryId", "CategoryName", iTems.CategoryID);
            return View(iTems);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                Guid depId = Guid.Parse(_keyManager.Unprotect(id));
                var iTems = await _context.Items.FindAsync(depId);
                if (iTems == null)
                {
                    return NotFound();
                }

                ViewData["EncryptedId"] = id;

                ViewData["CategoryID"] = new SelectList(_context.Categorys, "CategoryId", "CategoryName", iTems.CategoryID);
                return View(iTems);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string encryptedId, [Bind("ItemId,ItemName,Description,CategoryID,UnitPrice,ReoderLevel")] ITems item)
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

            if (decryptedId != item.ItemId)
                return BadRequest("Mismatched Item ID.");

            if (!ModelState.IsValid)
            {
                ViewData["CategoryID"] = new SelectList(_context.Categorys, "CategoryId", "CategoryName", item.CategoryID);
                return View(item);
            }

            try
            {
                var existingItem = await _context.Items.FindAsync(decryptedId);
                if (existingItem == null)
                    return NotFound();

                existingItem.ItemName = item.ItemName;
                existingItem.Description = item.Description;
                existingItem.CategoryID = item.CategoryID;
                existingItem.UnitPrice = item.UnitPrice;
                existingItem.ReoderLevel = item.ReoderLevel;

                _context.Update(existingItem);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ITemsExists(decryptedId))
                    return NotFound();

                throw;
            }
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var decryptedId = Guid.Parse(_keyManager.Unprotect(id));

            var iTems = await _context.Items
                .Include(i => i.Category)
                .FirstOrDefaultAsync(m => m.ItemId == decryptedId);
            if (iTems == null)
            {
                return NotFound();
            }

            return View(iTems);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var decryptedId = Guid.Parse(_keyManager.Unprotect(id));

            var iTems = await _context.Items.FindAsync(id);
            if (iTems != null)
            {
                _context.Items.Remove(iTems);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ITemsExists(Guid id)
        {
            return _context.Items.Any(e => e.ItemId == id);
        }
    }
}
