using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using TechWave_Electronics.Models;

namespace TechWave_Electronics.Models;

public partial class TechWaveElectronics : DbContext
{
    public TechWaveElectronics()
    {
    }

    public TechWaveElectronics(DbContextOptions<TechWaveElectronics> options)
        : base(options)
    {
    }


    public DbSet<Account> Account { get; set; }
    public DbSet<Income> Income { get; set; }
    public DbSet<Transaction> Transaction { get; set; }
    public DbSet<Portfolio> Portfolio { get; set; }
    public DbSet<Investment> Investment { get; set; }
    public DbSet<Security> Security { get; set; }   
    public DbSet<Expense> Expense { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<RecurringTransaction> RecurringTransaction { get; set; }
    public DbSet<Job> Job { get; set; }
    public DbSet<Attendance> Attendance { get; set; }
    public DbSet<Payroll> payrolls { get; set; }
    public DbSet<Leave> leaves { get; set; }
    public DbSet<Inventory> Inventory { get; set; }
    public DbSet<StockIn> StockIn { get; set; }
    public DbSet<StockOut> StockOut { get; set; }
    public DbSet<History> History { get; set; }
    public DbSet<Category> Categorys { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<TblCustomer> TblCustomers { get; set; }
    public DbSet<TblOrder> TblOrders { get; set; }
    public DbSet<TblOrderDetail> TblOrderDetails { get; set; }
    public DbSet<TblProduct> TblProducts { get; set; }
    public DbSet<TblShipper> TblShippers { get; set; }
    public DbSet<TblSupplier> TblSuppliers { get; set; }
    public DbSet<ITems> Items { get; set; }
    public DbSet<Warehouses> Warehouses { get; set; }
}
