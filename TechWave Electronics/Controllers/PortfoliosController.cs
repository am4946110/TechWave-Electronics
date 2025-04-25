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
    public class PortfoliosController : Controller
    {
        private readonly TechWaveElectronics _context;

        public PortfoliosController(TechWaveElectronics context)
        {
            _context = context;
        }

        // GET: Portfolios
        public async Task<IActionResult> Index()
        {
            var techWaveElectronics = _context.Portfolio.Include(p => p.Customer);
            return View(await techWaveElectronics.ToListAsync());
        }

        // GET: Portfolios/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolio = await _context.Portfolio
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(m => m.PortfolioID == id);
            if (portfolio == null)
            {
                return NotFound();
            }

            return View(portfolio);
        }

        // GET: Portfolios/Create
        public IActionResult Create()
        {
            ViewData["CustomerID"] = new SelectList(_context.TblCustomers, "CustomerId", "CustomerId");
            return View();
        }

        // POST: Portfolios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PortfolioID,CustomerID,Name,Description,CreatedDate,UpdatedDate")] Portfolio portfolio)
        {
            if (ModelState.IsValid)
            {
                portfolio.PortfolioID = Guid.NewGuid();
                _context.Add(portfolio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.TblCustomers, "CustomerId", "CustomerId", portfolio.CustomerID);
            return View(portfolio);
        }

        // GET: Portfolios/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolio = await _context.Portfolio.FindAsync(id);
            if (portfolio == null)
            {
                return NotFound();
            }
            ViewData["CustomerID"] = new SelectList(_context.TblCustomers, "CustomerId", "CustomerId", portfolio.CustomerID);
            return View(portfolio);
        }

        // POST: Portfolios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("PortfolioID,CustomerID,Name,Description,CreatedDate,UpdatedDate")] Portfolio portfolio)
        {
            if (id != portfolio.PortfolioID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(portfolio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PortfolioExists(portfolio.PortfolioID))
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
            ViewData["CustomerID"] = new SelectList(_context.TblCustomers, "CustomerId", "CustomerId", portfolio.CustomerID);
            return View(portfolio);
        }

        // GET: Portfolios/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolio = await _context.Portfolio
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(m => m.PortfolioID == id);
            if (portfolio == null)
            {
                return NotFound();
            }

            return View(portfolio);
        }

        // POST: Portfolios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var portfolio = await _context.Portfolio.FindAsync(id);
            if (portfolio != null)
            {
                _context.Portfolio.Remove(portfolio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PortfolioExists(Guid id)
        {
            return _context.Portfolio.Any(e => e.PortfolioID == id);
        }
    }
}
