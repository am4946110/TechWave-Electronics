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
    public class PayrollsController : Controller
    {
        private readonly TechWaveElectronics _context;
        private readonly ICustomKeyManager _keyManager;
        
        public PayrollsController(TechWaveElectronics context, ICustomKeyManager keyManager)
        {
            _context = context;
            _keyManager = keyManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var techWaveElectronics = _context.payrolls.Include(p => p.Employee)
                .ToList()
                .Select(p =>
                {
                    p.Id = _keyManager.Protect(p.PayrollID.ToString());
                    return p;
                });

            return View(techWaveElectronics);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                Guid  payrol = Guid.Parse(_keyManager.Unprotect(id));

                var payroll = await _context.payrolls
                       .Include(p => p.Employee)
                       .FirstOrDefaultAsync(m => m.PayrollID == payrol);

                if (payroll == null)
                {
                    return NotFound();
                }

                ViewData["EncryptedId"] = _keyManager.Protect(payroll.PayrollID.ToString());

                return View(payroll);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PayrollID,EmployeeId,Month,Year,BaseSalary,Deductions,Allowances,NetSalary")] Payroll payroll)
        {
            if (ModelState.IsValid)
            {
                payroll.PayrollID = Guid.NewGuid();
                _context.Add(payroll);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", payroll.EmployeeId);
            return View(payroll);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();


            try
            {
                Guid payrol = Guid.Parse(_keyManager.Unprotect(id));
                var payroll = await _context.payrolls.FindAsync(payrol);
                if (payroll == null)
                {
                    return NotFound();
                }
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", payroll.EmployeeId);
                return View(payroll);
            }
            catch (Exception)
            {
                return BadRequest("Invalid request.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PayrollID,EmployeeId,Month,Year,BaseSalary,Deductions,Allowances,NetSalary")] Payroll payroll)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", payroll.EmployeeId);
                return View(payroll);
            }

            try
            {
                Guid decryptedId = Guid.Parse(_keyManager.Unprotect(id));
                var existingPayroll = await _context.payrolls.FindAsync(decryptedId);

                if (existingPayroll == null)
                    return NotFound();

                existingPayroll.EmployeeId = payroll.EmployeeId;
                existingPayroll.Month = payroll.Month;
                existingPayroll.Year = payroll.Year;
                existingPayroll.BaseSalary = payroll.BaseSalary;
                existingPayroll.Deductions = payroll.Deductions;
                existingPayroll.Allowances = payroll.Allowances;
                existingPayroll.NetSalary = payroll.NetSalary;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PayrollExists(payroll.PayrollID))
                    return NotFound();
                else
                    return BadRequest("An error occurred while processing the request.");
            }
            catch
            {
                return BadRequest("Invalid request.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                Guid employeeId = Guid.Parse(_keyManager.Unprotect(id));

                var payroll = await _context.payrolls
                        .Include(p => p.Employee)
                        .FirstOrDefaultAsync(m => m.PayrollID == employeeId);
                if (payroll == null)
                {
                    return NotFound();
                }

                return View(payroll);
            }
            catch (Exception)
            {
                return BadRequest("Invalid request.");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string  id)
        {
            Guid employeeId = Guid.Parse(_keyManager.Unprotect(id));

            var payroll = await _context.payrolls.FindAsync(employeeId);
            if (payroll != null)
            {
                _context.payrolls.Remove(payroll);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PayrollExists(Guid id)
        {
            return _context.payrolls.Any(e => e.PayrollID == id);
        }
    }
}
