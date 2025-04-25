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
    public class IncomesController : Controller
    {
        private readonly TechWaveElectronics _context;

        public IncomesController(TechWaveElectronics context)
        {
            _context = context;
        }

        // GET: Incomes
        public async Task<IActionResult> Index()
        {
            var techWaveElectronics = _context.Income.Include(i => i.Account);
            return View(await techWaveElectronics.ToListAsync());
        }

        // GET: Incomes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var income = await _context.Income
                .Include(i => i.Account)
                .FirstOrDefaultAsync(m => m.IncomeID == id);
            if (income == null)
            {
                return NotFound();
            }

            return View(income);
        }

        // GET: Incomes/Create
        public IActionResult Create()
        {
            ViewData["AccountID"] = new SelectList(_context.Account, "AccountID", "AccountType");
            return View();
        }

        // POST: Incomes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IncomeID,AccountID,Source,Amount,IncomeDate,Description")] Income income)
        {
            if (ModelState.IsValid)
            {
                income.IncomeID = Guid.NewGuid();
                _context.Add(income);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountID"] = new SelectList(_context.Account, "AccountID", "AccountType", income.AccountID);
            return View(income);
        }

        // GET: Incomes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var income = await _context.Income.FindAsync(id);
            if (income == null)
            {
                return NotFound();
            }
            ViewData["AccountID"] = new SelectList(_context.Account, "AccountID", "AccountType", income.AccountID);
            return View(income);
        }

        // POST: Incomes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("IncomeID,AccountID,Source,Amount,IncomeDate,Description")] Income income)
        {
            if (id != income.IncomeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(income);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IncomeExists(income.IncomeID))
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
            ViewData["AccountID"] = new SelectList(_context.Account, "AccountID", "AccountType", income.AccountID);
            return View(income);
        }

        // GET: Incomes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var income = await _context.Income
                .Include(i => i.Account)
                .FirstOrDefaultAsync(m => m.IncomeID == id);
            if (income == null)
            {
                return NotFound();
            }

            return View(income);
        }

        // POST: Incomes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var income = await _context.Income.FindAsync(id);
            if (income != null)
            {
                _context.Income.Remove(income);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IncomeExists(Guid id)
        {
            return _context.Income.Any(e => e.IncomeID == id);
        }
    }
}
