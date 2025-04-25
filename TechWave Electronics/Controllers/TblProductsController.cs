using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechWave_Electronics.Controllers;
using TechWave_Electronics.Models;

namespace TechWave_Electronics.Controllers
{
    [ActivityLog]
    public class TblProductsController : Controller
    {
        private readonly TechWaveElectronics _context;
        private readonly ICustomKeyManager _keyManager;

        public TblProductsController(ICustomKeyManager keyManager, TechWaveElectronics context)
        {
            _context = context;
            _keyManager = keyManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _context.TblProducts
                .Include(t => t.Category)
                .Include(t => t.Supplier)
                .ToListAsync();

            var protectedProducts = products.Select(p =>
            {
                p.Id = _keyManager.Protect(p.ProductId.ToString());
                return p;
            });

            return View(protectedProducts);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            try
            {
                var productId = Guid.Parse(_keyManager.Unprotect(id));

                var product = await _context.TblProducts
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .FirstOrDefaultAsync(p => p.ProductId == productId);

                if (product == null)
                    return NotFound();

                ViewData["EncryptedId"] = id;
                return View(product);
            }
            catch
            {
                return BadRequest("Invalid ID.");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ImageFile,SupplierId,CategoryId,ProductName,EnglishName,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued")] TblProduct tblProduct)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns(tblProduct.CategoryId, tblProduct.SupplierId);

                tblProduct.ProductId = Guid.NewGuid();

                if (tblProduct.ImageFile != null && tblProduct.ImageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await tblProduct.ImageFile.CopyToAsync(memoryStream);
                        tblProduct.itemImg = memoryStream.ToArray();
                    }
                }

                _context.Add(tblProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(tblProduct);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            try
            {
                // فك تشفير الـ ID وتحويله إلى Guid
                var productId = Guid.Parse(_keyManager.Unprotect(id));
                var product = await _context.TblProducts.FindAsync(productId);

                if (product == null)
                    return NotFound();

                ViewData["EncryptedId"] = id;
                // تعبئة عناصر القائمة المنسدلة بناءً على بيانات الفئة والمورد
                PopulateDropdowns(product.CategoryId, product.SupplierId);
                return View(product);
            }
            catch
            {
                return BadRequest("Invalid request.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ProductId,ImageFile,SupplierId,CategoryId,ProductName,EnglishName,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued")] TblProduct tblProduct)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            // فك تشفير الـ ID المرسل من النموذج
            Guid decryptedId;
            try
            {
                decryptedId = Guid.Parse(_keyManager.Unprotect(id));
            }
            catch
            {
                return BadRequest("Invalid ID.");
            }

            var productInDb = await _context.TblProducts.FindAsync(decryptedId);

            try
            {
                if (productInDb != null)
                {
                    productInDb.SupplierId = tblProduct.SupplierId;
                    productInDb.CategoryId = tblProduct.CategoryId;
                    productInDb.ProductName = tblProduct.ProductName;
                    productInDb.EnglishName = tblProduct.EnglishName;
                    productInDb.QuantityPerUnit = tblProduct.QuantityPerUnit;
                    productInDb.UnitPrice = tblProduct.UnitPrice;
                    productInDb.UnitsInStock = tblProduct.UnitsInStock;
                    productInDb.UnitsOnOrder = tblProduct.UnitsOnOrder;
                    productInDb.ReorderLevel = tblProduct.ReorderLevel;
                    productInDb.Discontinued = tblProduct.Discontinued;

                    // تحديث الصورة في حال تم رفع ملف صورة جديد
                    if (tblProduct.ImageFile != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await tblProduct.ImageFile.CopyToAsync(memoryStream);
                            productInDb.itemImg = memoryStream.ToArray();
                        }
                    }

                    // حفظ التغييرات في قاعدة البيانات
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
               
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblProductExists(tblProduct.ProductId))
                    return NotFound();
                throw;
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Failed to save changes. Please try again.");
            }

            // في حالة حدوث خطأ إعادة تعبئة عناصر القائمة المنسدلة وعرض النموذج مرة أخرى
            ViewData["EncryptedId"] = id;
            PopulateDropdowns(tblProduct.CategoryId, tblProduct.SupplierId);
            return View(tblProduct);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            try
            {
                var productId = Guid.Parse(_keyManager.Unprotect(id));

                var product = await _context.TblProducts
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .FirstOrDefaultAsync(p => p.ProductId == productId);

                if (product == null)
                    return NotFound();

                ViewData["EncryptedId"] = id;
                return View(product);
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
                var productId = Guid.Parse(_keyManager.Unprotect(id));

                var product = await _context.TblProducts.FindAsync(productId);
                if (product != null)
                {
                    _context.TblProducts.Remove(product);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return BadRequest("Failed to delete product.");
            }
        }

        private bool TblProductExists(Guid id)
        {
            return _context.TblProducts.Any(e => e.ProductId == id);
        }

        private void PopulateDropdowns(Guid? selectedCategory = null, Guid? selectedSupplier = null)
        {
            ViewData["CategoryId"] = new SelectList(_context.Categorys, "CategoryId", "CategoryName", selectedCategory);
            ViewData["SupplierId"] = new SelectList(_context.TblSuppliers, "SupplierId", "Name", selectedSupplier);
        }
    }
}
