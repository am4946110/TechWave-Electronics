using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechWave_Electronics.Models;

namespace TechWave_Electronics.Controllers
{
    public class TblOrdersController : Controller
    {
        private readonly TechWaveElectronics _context;
        private readonly ICustomKeyManager _keyManager;

        public TblOrdersController(ICustomKeyManager keyManager, TechWaveElectronics context)
        {
            _context = context;
            _keyManager = keyManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await _context.TblOrders
                .Include(t => t.Customer)
                .Include(t => t.Employee)
                .Include(t => t.ShipViaNavigationShipper)
                .ToListAsync();

            var protectedOrders = orders.Select(o =>
            {
                o.Id = _keyManager.Protect(o.OrderId.ToString());
                return o;
            });

            return View(protectedOrders);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                var orderId = Guid.Parse(_keyManager.Unprotect(id));
                var order = await _context.TblOrders
                    .Include(t => t.Customer)
                    .Include(t => t.Employee)
                    .Include(t => t.ShipViaNavigationShipper)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null) return NotFound();

                ViewData["EncryptedId"] = id;
                return View(order);
            }
            catch (CryptographicException)
            {
                return BadRequest("Invalid ID.");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.TblCustomers, "CustomerId", "CustomerId");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            ViewData["ShipViaNavigationShipperId"] = new SelectList(_context.TblShippers, "ShipperId", "ShipperId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,CustomerId,EmployeeId,Name,Address,City,Region,PostalCode,Country,ShipVia,OrderDate,RequiredDate,ShippedDate,Freight,ShipViaNavigationShipperId")] TblOrder tblOrder)
        {
            if (!ModelState.IsValid) return View(tblOrder);

            tblOrder.OrderId = Guid.NewGuid();
            _context.Add(tblOrder);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                var decryptedId = Guid.Parse(_keyManager.Unprotect(id));
                var order = await _context.TblOrders.FindAsync(decryptedId);
                if (order == null) return NotFound();

                ViewData["EncryptedId"] = id;
                ViewData["CustomerId"] = new SelectList(_context.TblCustomers, "CustomerId", "CustomerId", order.CustomerId);
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", order.EmployeeId);
                ViewData["ShipViaNavigationShipperId"] = new SelectList(_context.TblShippers, "ShipperId", "ShipperId", order.ShipViaNavigationShipperId);
                return View(order);
            }
            catch
            {
                return BadRequest("Invalid request.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("OrderId,CustomerId,EmployeeId,Name,Address,City,Region,PostalCode,Country,ShipVia,OrderDate,RequiredDate,ShippedDate,Freight,ShipViaNavigationShipperId")] TblOrder tblOrder)
        {
            if (!ModelState.IsValid) return View(tblOrder);

            try
            {
                var decryptedId = Guid.Parse(_keyManager.Unprotect(id));
                if (decryptedId != tblOrder.OrderId)
                    return NotFound();

                _context.Update(tblOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblOrderExists(tblOrder.OrderId))
                    return NotFound();
                throw;
            }
            catch
            {
                ModelState.AddModelError("", "Could not save changes.");
            }

            ViewData["EncryptedId"] = id;
            ViewData["CustomerId"] = new SelectList(_context.TblCustomers, "CustomerId", "CustomerId", tblOrder.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", tblOrder.EmployeeId);
            ViewData["ShipViaNavigationShipperId"] = new SelectList(_context.TblShippers, "ShipperId", "ShipperId", tblOrder.ShipViaNavigationShipperId);
            return View(tblOrder);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                var orderId = Guid.Parse(_keyManager.Unprotect(id));
                var order = await _context.TblOrders
                    .Include(t => t.Customer)
                    .Include(t => t.Employee)
                    .Include(t => t.ShipViaNavigationShipper)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null) return NotFound();

                return View(order);
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
                var orderId = Guid.Parse(_keyManager.Unprotect(id));
                var order = await _context.TblOrders.FindAsync(orderId);
                if (order != null)
                {
                    _context.TblOrders.Remove(order);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return BadRequest("Error during deletion.");
            }
        }

        private bool TblOrderExists(Guid id)
        {
            return _context.TblOrders.Any(e => e.OrderId == id);
        }
    }
}
