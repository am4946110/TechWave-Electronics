using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TechWave_Electronics.Models;

namespace TechWave_Electronics.Controllers
{
    public class TblOrderDetailsController : Controller
    {
        private readonly TechWaveElectronics _context;

        private readonly ICustomKeyManager _keyManager;

        public TblOrderDetailsController(TechWaveElectronics context, ICustomKeyManager keyManager)
        {
            _context = context;
            _keyManager = keyManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orderDetails = await _context.TblOrderDetails
                .Include(t => t.Product)
                .ToListAsync();

            var protectedOrderDetails = orderDetails.Select(od =>
            {
                od.Id = _keyManager.Protect(od.OrderId.ToString());
                return od;
            });

            return View(protectedOrderDetails);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            Guid OrderId;
            try
            {
                string unprotectedId = _keyManager.Unprotect(id);
                if (!Guid.TryParse(unprotectedId, out OrderId))
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

            var tblOrderDetail = await _context.TblOrderDetails
                .Include(t => t.Product)
                .FirstOrDefaultAsync(m => m.OrderId == OrderId);
            if (tblOrderDetail == null)
            {
                return NotFound();
            }

            ViewData["EncryptedId"] = _keyManager.Protect(tblOrderDetail.OrderId.ToString());


            return View(tblOrderDetail);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.TblProducts, "ProductId", "ProductId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,ProductId,UnitPrice,Quantity,Discount,Tax,Total,OrderId1")] TblOrderDetail tblOrderDetail)
        {
            if (tblOrderDetail != null)
            {
                tblOrderDetail.OrderId = Guid.NewGuid();
                _context.Add(tblOrderDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.TblProducts, "ProductId", "ProductId", tblOrderDetail.ProductId);
            return View(tblOrderDetail);
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
                Guid audienceId = Guid.Parse(_keyManager.Unprotect(id));

                var tblOrderDetail = await _context.TblOrderDetails.FindAsync(audienceId);
                if (tblOrderDetail == null)
                {
                    return NotFound();
                }
                ViewData["EncryptedId"] = _keyManager.Protect(tblOrderDetail.OrderId.ToString());

                ViewData["ProductId"] = new SelectList(_context.TblProducts, "ProductId", "ProductId", tblOrderDetail.ProductId);
                return View(tblOrderDetail);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("OrderId,ProductId,UnitPrice,Quantity,Discount,Tax,Total,OrderId1")] TblOrderDetail tblOrderDetail)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid request.");
            }

            Guid decryptedId;
            try
            {
                decryptedId = Guid.Parse(_keyManager.Unprotect(id));
            }
            catch
            {
                return BadRequest("Invalid request.");
            }

            if (decryptedId != tblOrderDetail.OrderId)
            {
                return BadRequest("Mismatched ID.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblOrderDetail);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblOrderDetailExists(tblOrderDetail.OrderId))
                    {
                        return NotFound();
                    }

                    throw;
                }
                catch
                {
                    ModelState.AddModelError("", "حدث خطأ أثناء حفظ التغييرات.");
                }
            }

            ViewData["EncryptedId"] = id;
            ViewData["ProductId"] = new SelectList(_context.TblProducts, "ProductId", "ProductId", tblOrderDetail.ProductId);
            return View(tblOrderDetail);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Invalid request.");

            try
            {
                Guid decryptedId = Guid.Parse(_keyManager.Unprotect(id));
                var orderDetail = await _context.TblOrderDetails
                    .Include(t => t.Product)
                    .FirstOrDefaultAsync(m => m.OrderId == decryptedId);

                if (orderDetail == null)
                    return NotFound();

                return View(orderDetail);
            }
            catch
            {
                return BadRequest("Invalid request.");
            }
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            Guid id1 = Guid.Parse(_keyManager.Unprotect(id));

            var tblOrderDetail = await _context.TblOrderDetails.FindAsync(id1);
            if (tblOrderDetail != null)
            {
                _context.TblOrderDetails.Remove(tblOrderDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblOrderDetailExists(Guid id)
        {
            return _context.TblOrderDetails.Any(e => e.OrderId == id);
        }
    }
}
