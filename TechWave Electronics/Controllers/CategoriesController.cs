using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TechWave_Electronics.Models;

namespace TechWave_Electronics.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly TechWaveElectronics _context;
        private readonly ICustomKeyManager _keyManager;

        public CategoriesController(TechWaveElectronics context, ICustomKeyManager keyManager)
        {
            _context = context;
            _keyManager = keyManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // جلب الفئات من قاعدة البيانات وتشفير الـ GUID لكل فئة
            var categories = await _context.Categorys.ToListAsync();
            var encryptedCategories = categories.Select(c =>
            {
                c.Id = _keyManager.Protect(c.CategoryId.ToString());
                return c;
            }).ToList();

            return View(encryptedCategories);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            Guid categoryId;
            try
            {
                string decryptedId = _keyManager.Unprotect(id);
                if (!Guid.TryParse(decryptedId, out categoryId))
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

            var category = await _context.Categorys.FirstOrDefaultAsync(c => c.CategoryId == categoryId);
            if (category == null)
            {
                return NotFound();
            }

            // إعادة تشفير المعرف لعرضه في الصفحة إذا لزم الأمر
            ViewData["EncryptedId"] = _keyManager.Protect(category.CategoryId.ToString());
            return View(category);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName")] Category category)
        {
            // التحقق من صحة البيانات المُدخلة
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            // إنشاء معرف جديد للفئة قبل الحفظ
            category.CategoryId = Guid.NewGuid();
            _context.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                Guid categoryId = Guid.Parse(_keyManager.Unprotect(id));
                var category = await _context.Categorys.FindAsync(categoryId);
                if (category == null)
                {
                    return NotFound();
                }
                return View(category);
            }
            catch (Exception)
            {
                return BadRequest("Invalid ID format.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CategoryId,CategoryName")] Category category)
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
            catch
            {
                return BadRequest("Invalid ID format.");
            }

            if (category.CategoryId != decryptedId)
            {
                return BadRequest("Mismatched category ID.");
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.CategoryId))
                {
                    return NotFound();
                }
                throw;
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact support.");
                return View(category);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            Guid categoryId;
            try
            {
                categoryId = Guid.Parse(_keyManager.Unprotect(id));
            }
            catch
            {
                return BadRequest("Invalid ID format.");
            }

            var category = await _context.Categorys.FirstOrDefaultAsync(c => c.CategoryId == categoryId);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            Guid categoryId;
            try
            {
                categoryId = Guid.Parse(_keyManager.Unprotect(id));
            }
            catch
            {
                return BadRequest("Invalid ID format.");
            }

            var category = await _context.Categorys.FindAsync(categoryId);
            if (category != null)
            {
                _context.Categorys.Remove(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(Guid id)
        {
            return _context.Categorys.Any(e => e.CategoryId == id);
        }
    }
}
