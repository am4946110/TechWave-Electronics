using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechWave_Electronics.Models;


namespace TechWave_Electronics.Controllers
{
    public class TblCustomersController : Controller
    {
        private readonly TechWaveElectronics _context;

        private readonly ICustomKeyManager _keyManager;

        public TblCustomersController(ICustomKeyManager keyManager, TechWaveElectronics context)
        {
            _context = context;

            _keyManager = keyManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var customers = await _context.TblCustomers.ToListAsync();

            // تشفير المعرفات
            var protectedCustomers = customers.Select(c =>
            {
                c.Id = _keyManager.Protect(c.CustomerId.ToString());
                return c;
            }).ToList();

            return View(protectedCustomers);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                var decryptedId = Guid.Parse(_keyManager.Unprotect(id));
                var customer = await _context.TblCustomers.FirstOrDefaultAsync(c => c.CustomerId == decryptedId);
                if (customer == null) return NotFound();

                return View(customer);
            }
            catch
            {
                return BadRequest("Invalid ID format.");
            }
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,CreatedAt,UpdatedAt,Active,CompanyName,FirstName,LastName,ContactTitle,Address,City,Region,PostalCode,Country,Phone,Email,Fax,ImageFile")] TblCustomer tblCustomer)
        {
            if (ModelState.IsValid)
            {
                if (tblCustomer.ImageFile != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await tblCustomer.ImageFile.CopyToAsync(memoryStream);
                        tblCustomer.CustomerUrl = memoryStream.ToArray();
                    }
                }

                tblCustomer.CustomerId = Guid.NewGuid();
                _context.Add(tblCustomer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(tblCustomer);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Invalid request.");

            try
            {
                var decryptedId = Guid.Parse(_keyManager.Unprotect(id));
                var customer = await _context.TblCustomers.FindAsync(decryptedId);
                if (customer == null) return NotFound();

                return View(customer);
            }
            catch
            {
                return BadRequest("Invalid request.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CustomerId,CreatedAt,UpdatedAt,Active,CompanyName,FirstName,LastName,ContactTitle,Address,City,Region,PostalCode,Country,Phone,Email,Fax,ImageFile")] TblCustomer tblCustomer)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid request.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (tblCustomer.ImageFile != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await tblCustomer.ImageFile.CopyToAsync(memoryStream);
                            tblCustomer.CustomerUrl = memoryStream.ToArray();
                        }
                    }
                    _context.Update(tblCustomer);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblCustomerExists(tblCustomer.CustomerId))
                        return NotFound();
                    else
                        throw;
                }
                catch
                {
                    return BadRequest("An error occurred while updating the customer.");
                }
            }

            return View(tblCustomer);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                var decryptedId = Guid.Parse(_keyManager.Unprotect(id));
                var customer = await _context.TblCustomers.FirstOrDefaultAsync(c => c.CustomerId == decryptedId);
                if (customer == null) return NotFound();

                return View(customer);
            }
            catch
            {
                return BadRequest("Invalid ID format.");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var decryptedId = Guid.Parse(_keyManager.Unprotect(id));
                var customer = await _context.TblCustomers.FindAsync(decryptedId);
                if (customer != null)
                {
                    _context.TblCustomers.Remove(customer);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return BadRequest("Invalid delete request.");
            }
        }

        private bool TblCustomerExists(Guid id) =>
            _context.TblCustomers.Any(e => e.CustomerId == id);
    }
}
