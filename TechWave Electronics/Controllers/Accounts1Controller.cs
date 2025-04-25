using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechWave_Electronics.Models;

namespace TechWave_Electronics.Controllers
{
    public class Accounts1Controller : Controller
    {
        private readonly TechWaveElectronics _context;

        public Accounts1Controller(TechWaveElectronics context)
        {
            _context = context;
        }

        // GET: Accounts1
        public async Task<IActionResult> Index()
        {
            var techWaveElectronics = _context.Account.Include(a => a.Customer);
            return View(await techWaveElectronics.ToListAsync());
        }

        // GET: Accounts1/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(m => m.AccountID == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Accounts1/Create
        public IActionResult Create()
        {
            ViewData["CustomerID"] = new SelectList(_context.TblCustomers, "CustomerId", "CustomerId");
            return View();
        }

        // POST: Accounts1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountID,CustomerID,AccountType,Currency,Balance,DateOpened,DateClosed,Status")] Account account)
        {
            if (ModelState.IsValid)
            {
                account.AccountID = Guid.NewGuid();
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.TblCustomers, "CustomerId", "CustomerId", account.CustomerID);
            return View(account);
        }

        // GET: Accounts1/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["CustomerID"] = new SelectList(_context.TblCustomers, "CustomerId", "CustomerId", account.CustomerID);
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("AccountID,CustomerID,AccountType,Currency,Balance,DateOpened,DateClosed,Status")] Account account)
        {
            if (id != account.AccountID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.TblCustomers, "CustomerId", "CustomerId", account.CustomerID);
            return View(account);
        }

        // GET: Accounts1/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(m => m.AccountID == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var account = await _context.Account.FindAsync(id);
            if (account != null)
            {
                _context.Account.Remove(account);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(Guid id)
        {
            return _context.Account.Any(e => e.AccountID == id);
        }
    }
}
