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
    public class InvestmentsController : Controller
    {
        private readonly TechWaveElectronics _context;

        public InvestmentsController(TechWaveElectronics context)
        {
            _context = context;
        }

        // GET: Investments
        public async Task<IActionResult> Index()
        {
            var techWaveElectronics = _context.Investment.Include(i => i.Portfolio).Include(i => i.Security);
            return View(await techWaveElectronics.ToListAsync());
        }

        // GET: Investments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var investment = await _context.Investment
                .Include(i => i.Portfolio)
                .Include(i => i.Security)
                .FirstOrDefaultAsync(m => m.InvestmentID == id);
            if (investment == null)
            {
                return NotFound();
            }

            return View(investment);
        }

        // GET: Investments/Create
        public IActionResult Create()
        {
            ViewData["PortfolioID"] = new SelectList(_context.Portfolio, "PortfolioID", "Name");
            ViewData["SecurityID"] = new SelectList(_context.Security, "SecurityID", "Currency");
            return View();
        }

        // POST: Investments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InvestmentID,PortfolioID,SecurityID,Quantity,PurchaseDate,PurchasePrice,CurrentValue")] Investment investment)
        {
            if (ModelState.IsValid)
            {
                investment.InvestmentID = Guid.NewGuid();
                _context.Add(investment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PortfolioID"] = new SelectList(_context.Portfolio, "PortfolioID", "Name", investment.PortfolioID);
            ViewData["SecurityID"] = new SelectList(_context.Security, "SecurityID", "Currency", investment.SecurityID);
            return View(investment);
        }

        // GET: Investments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var investment = await _context.Investment.FindAsync(id);
            if (investment == null)
            {
                return NotFound();
            }
            ViewData["PortfolioID"] = new SelectList(_context.Portfolio, "PortfolioID", "Name", investment.PortfolioID);
            ViewData["SecurityID"] = new SelectList(_context.Security, "SecurityID", "Currency", investment.SecurityID);
            return View(investment);
        }

        // POST: Investments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("InvestmentID,PortfolioID,SecurityID,Quantity,PurchaseDate,PurchasePrice,CurrentValue")] Investment investment)
        {
            if (id != investment.InvestmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(investment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvestmentExists(investment.InvestmentID))
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
            ViewData["PortfolioID"] = new SelectList(_context.Portfolio, "PortfolioID", "Name", investment.PortfolioID);
            ViewData["SecurityID"] = new SelectList(_context.Security, "SecurityID", "Currency", investment.SecurityID);
            return View(investment);
        }

        // GET: Investments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var investment = await _context.Investment
                .Include(i => i.Portfolio)
                .Include(i => i.Security)
                .FirstOrDefaultAsync(m => m.InvestmentID == id);
            if (investment == null)
            {
                return NotFound();
            }

            return View(investment);
        }

        // POST: Investments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var investment = await _context.Investment.FindAsync(id);
            if (investment != null)
            {
                _context.Investment.Remove(investment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvestmentExists(Guid id)
        {
            return _context.Investment.Any(e => e.InvestmentID == id);
        }
    }
}
