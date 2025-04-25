using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechWave_Electronics.Models;

namespace TechWave_Electronics.Controllers
{
    public class InventoriesController : Controller
    {
        private readonly TechWaveElectronics _context;
        private readonly ICustomKeyManager _keyManager;

        public InventoriesController(TechWaveElectronics context, ICustomKeyManager keyManager)
        {
            _context = context;
            _keyManager = keyManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var inventories = await _context.Inventory.ToListAsync();

            foreach (var item in inventories)
            {
                item.Id = _keyManager.Protect(item.InventoryId.ToString());
            }

            return View(inventories);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            if (!TryDecryptId(id, out Guid inventoryId))
                return BadRequest("Invalid ID format.");

            var inventory = await _context.Inventory.FirstOrDefaultAsync(m => m.InventoryId == inventoryId);
            if (inventory == null)
                return NotFound();

            ViewData["EncryptedId"] = _keyManager.Protect(inventory.InventoryId.ToString());
            return View(inventory);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,WarehouseId,Quantity")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                inventory.InventoryId = Guid.NewGuid();
                _context.Add(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(inventory);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (!TryDecryptId(id, out Guid inventoryId))
                return BadRequest("Invalid ID format.");

            var inventory = await _context.Inventory.FindAsync(inventoryId);
            if (inventory == null)
                return NotFound();

            ViewData["EncryptedId"] = id;
            return View(inventory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("InventoryId,ItemId,WarehouseId,Quantity")] Inventory inventory)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            if (!TryDecryptId(id, out Guid decryptedId))
                return BadRequest("Invalid ID format.");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingInventory = await _context.Inventory.FindAsync(decryptedId);
                    if (existingInventory == null)
                        return NotFound();

                    // تحديث الخصائص المطلوبة فقط
                    existingInventory.ItemId = inventory.ItemId;
                    existingInventory.WarehouseId = inventory.WarehouseId;
                    existingInventory.Quantity = inventory.Quantity;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryExists(decryptedId))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["EncryptedId"] = id;
            return View(inventory);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (!TryDecryptId(id, out Guid inventoryId))
                return BadRequest("Invalid ID format.");

            var inventory = await _context.Inventory
                .FirstOrDefaultAsync(m => m.InventoryId == inventoryId);
            if (inventory == null)
                return NotFound();

            return View(inventory);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var inventory = await _context.Inventory.FindAsync(id);
            if (inventory != null)
            {
                _context.Inventory.Remove(inventory);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool InventoryExists(Guid id)
        {
            return _context.Inventory.Any(e => e.InventoryId == id);
        }

        private bool TryDecryptId(string encryptedId, out Guid guid)
        {
            guid = Guid.Empty;
            try
            {
                string decrypted = _keyManager.Unprotect(encryptedId);
                return Guid.TryParse(decrypted, out guid);
            }
            catch (CryptographicException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
