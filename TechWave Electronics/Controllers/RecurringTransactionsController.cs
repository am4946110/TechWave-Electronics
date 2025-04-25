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
    public class RecurringTransactionsController : Controller
    {
        private readonly TechWaveElectronics _context;

        public RecurringTransactionsController(TechWaveElectronics context)
        {
            _context = context;
        }

        // GET: RecurringTransactions
        public async Task<IActionResult> Index()
        {
            var techWaveElectronics = _context.RecurringTransaction.Include(r => r.Account);
            return View(await techWaveElectronics.ToListAsync());
        }

        // GET: RecurringTransactions/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recurringTransaction = await _context.RecurringTransaction
                .Include(r => r.Account)
                .FirstOrDefaultAsync(m => m.RecurringTransactionID == id);
            if (recurringTransaction == null)
            {
                return NotFound();
            }

            return View(recurringTransaction);
        }

        // GET: RecurringTransactions/Create
        public IActionResult Create()
        {
            ViewData["AccountID"] = new SelectList(_context.Account, "AccountID", "AccountType");
            return View();
        }

        // POST: RecurringTransactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecurringTransactionID,AccountID,TransactionType,Amount,Currency,Frequency,StartDate,EndDate,NextExecutionDate")] RecurringTransaction recurringTransaction)
        {
            if (ModelState.IsValid)
            {
                recurringTransaction.RecurringTransactionID = Guid.NewGuid();
                _context.Add(recurringTransaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountID"] = new SelectList(_context.Account, "AccountID", "AccountType", recurringTransaction.AccountID);
            return View(recurringTransaction);
        }

        // GET: RecurringTransactions/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recurringTransaction = await _context.RecurringTransaction.FindAsync(id);
            if (recurringTransaction == null)
            {
                return NotFound();
            }
            ViewData["AccountID"] = new SelectList(_context.Account, "AccountID", "AccountType", recurringTransaction.AccountID);
            return View(recurringTransaction);
        }

        // POST: RecurringTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("RecurringTransactionID,AccountID,TransactionType,Amount,Currency,Frequency,StartDate,EndDate,NextExecutionDate")] RecurringTransaction recurringTransaction)
        {
            if (id != recurringTransaction.RecurringTransactionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recurringTransaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecurringTransactionExists(recurringTransaction.RecurringTransactionID))
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
            ViewData["AccountID"] = new SelectList(_context.Account, "AccountID", "AccountType", recurringTransaction.AccountID);
            return View(recurringTransaction);
        }

        // GET: RecurringTransactions/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recurringTransaction = await _context.RecurringTransaction
                .Include(r => r.Account)
                .FirstOrDefaultAsync(m => m.RecurringTransactionID == id);
            if (recurringTransaction == null)
            {
                return NotFound();
            }

            return View(recurringTransaction);
        }

        // POST: RecurringTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var recurringTransaction = await _context.RecurringTransaction.FindAsync(id);
            if (recurringTransaction != null)
            {
                _context.RecurringTransaction.Remove(recurringTransaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecurringTransactionExists(Guid id)
        {
            return _context.RecurringTransaction.Any(e => e.RecurringTransactionID == id);
        }
    }
}
