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
    public class SecuritiesController : Controller
    {
        private readonly TechWaveElectronics _context;

        public SecuritiesController(TechWaveElectronics context)
        {
            _context = context;
        }

        // GET: Securities
        public async Task<IActionResult> Index()
        {
            return View(await _context.Security.ToListAsync());
        }

        // GET: Securities/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var security = await _context.Security
                .FirstOrDefaultAsync(m => m.SecurityID == id);
            if (security == null)
            {
                return NotFound();
            }

            return View(security);
        }

        // GET: Securities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Securities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SecurityID,SecurityType,Symbol,Name,MarketValue,Currency,LastUpdated")] Security security)
        {
            if (ModelState.IsValid)
            {
                security.SecurityID = Guid.NewGuid();
                _context.Add(security);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(security);
        }

        // GET: Securities/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var security = await _context.Security.FindAsync(id);
            if (security == null)
            {
                return NotFound();
            }
            return View(security);
        }

        // POST: Securities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("SecurityID,SecurityType,Symbol,Name,MarketValue,Currency,LastUpdated")] Security security)
        {
            if (id != security.SecurityID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(security);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SecurityExists(security.SecurityID))
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
            return View(security);
        }

        // GET: Securities/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var security = await _context.Security
                .FirstOrDefaultAsync(m => m.SecurityID == id);
            if (security == null)
            {
                return NotFound();
            }

            return View(security);
        }

        // POST: Securities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var security = await _context.Security.FindAsync(id);
            if (security != null)
            {
                _context.Security.Remove(security);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SecurityExists(Guid id)
        {
            return _context.Security.Any(e => e.SecurityID == id);
        }
    }
}
