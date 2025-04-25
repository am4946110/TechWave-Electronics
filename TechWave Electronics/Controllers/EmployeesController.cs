using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechWave_Electronics.Models;

namespace TechWave_Electronics.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly TechWaveElectronics _context;
        private readonly ICustomKeyManager _keyManager;

        public EmployeesController(TechWaveElectronics context, ICustomKeyManager keyManager)
        {
            _context = context;
            _keyManager = keyManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Include(e => e.Departments)
                .Include(e => e.Job)
                .ToListAsync();

            var encryptedEmployees = employees.Select(e =>
            {
                e.Id = _keyManager.Protect(e.EmployeeId.ToString());
                return e;
            });

            return View(encryptedEmployees);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                Guid employeeId = Guid.Parse(_keyManager.Unprotect(id));

                var employee = await _context.Employees
                    .Include(e => e.Departments)
                    .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

                if (employee == null) return NotFound();

                ViewData["EncryptedId"] = _keyManager.Protect(employee.EmployeeId.ToString());

                return View(employee);
            }
            catch
            {
                return BadRequest("Invalid ID format.");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["DepartmentsId"] = new SelectList(_context.Departments, "DepartmentsId", "DepartmentsId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,JobID,ImageFile,FirstName,LastName,HireDate,BirthDate,Address,City,Email,Phone,Country,Salary,Incentive,DepartmentsId")] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                ViewData["DepartmentsId"] = new SelectList(_context.Departments, "DepartmentsId", "DepartmentsId", employee.DepartmentsId);
                return View(employee);
            }

            employee.EmployeeId = Guid.NewGuid();

            if (employee.ImageFile != null)
            {
                using var ms = new MemoryStream();
                await employee.ImageFile.CopyToAsync(ms);
                employee.Employeeurl = ms.ToArray();
            }

            _context.Add(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                Guid employeeId = Guid.Parse(_keyManager.Unprotect(id));

                var employee = await _context.Employees.FindAsync(employeeId);
                if (employee == null) return NotFound();

                ViewData["DepartmentsId"] = new SelectList(_context.Departments, "DepartmentsId", "DepartmentsId", employee.DepartmentsId);
                return View(employee);
            }
            catch
            {
                return BadRequest("Invalid request.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("EmployeeId,JobID,ImageFile,FirstName,LastName,HireDate,BirthDate,Address,City,Email,Phone,Country,Salary,Incentive,DepartmentsId")] Employee employee)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["DepartmentsId"] = new SelectList(_context.Departments, "DepartmentsId", "DepartmentsId", employee.DepartmentsId);
                return View(employee);
            }

            try
            {
                Guid employeeId = Guid.Parse(_keyManager.Unprotect(id));
                var existingEmployee = await _context.Employees.FindAsync(employeeId);

                if (existingEmployee == null) return NotFound();

                if (employee.ImageFile != null)
                {
                    using var ms = new MemoryStream();
                    await employee.ImageFile.CopyToAsync(ms);
                    existingEmployee.Employeeurl = ms.ToArray();
                }
          
                existingEmployee.FirstName = employee.FirstName;
                existingEmployee.LastName = employee.LastName;
                existingEmployee.HireDate = employee.HireDate;
                existingEmployee.BirthDate = employee.BirthDate;
                existingEmployee.Address = employee.Address;
                existingEmployee.City = employee.City;
                existingEmployee.Email = employee.Email;
                existingEmployee.Phone = employee.Phone;
                existingEmployee.Country = employee.Country;
                existingEmployee.Salary = employee.Salary;
                existingEmployee.Incentive = employee.Incentive;
                existingEmployee.DepartmentsId = employee.DepartmentsId;

            
               
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return BadRequest("An error occurred while processing the request.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            try
            {
                Guid employeeId = Guid.Parse(_keyManager.Unprotect(id));

                var employee = await _context.Employees
                    .Include(e => e.Departments)
                    .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

                if (employee == null) return NotFound();

                return View(employee);
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
            Guid employeeId = Guid.Parse(_keyManager.Unprotect(id));

            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(Guid id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
