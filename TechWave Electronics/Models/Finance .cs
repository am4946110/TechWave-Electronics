using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechWave_Electronics.Models
{
    public class Account
    {
        [NotMapped]
        public string? Id { get; set; }

        [Key]
        public Guid AccountID { get; set; } = Guid.NewGuid();

        [Required]
        public Guid CustomerID { get; set; }

        [Required, MaxLength(20)]
        public string AccountType { get; set; } = string.Empty;

        [Required, StringLength(3)]
        public string Currency { get; set; } = "";  

        [Column(TypeName = "decimal(10,2)")]
        public decimal Balance { get; set; }

        [Required]
        public DateOnly DateOpened { get; set; }

        public DateOnly? DateClosed { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Active";

        public TblCustomer Customer { get; set; } = null!;
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public virtual ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
        public virtual ICollection<Income> Incomes { get; set; } = new List<Income>();
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public virtual ICollection<RecurringTransaction> RecurringTransactions { get; set; } = new List<RecurringTransaction>();
    }

    public class Income
    {
        [NotMapped]
        public string? Id { get; set; }

        [Key]
        public Guid IncomeID { get; set; } = Guid.NewGuid();

        [Required]
        public Guid AccountID { get; set; }

        [Required, MaxLength(100)]
        public string Source { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateOnly IncomeDate { get; set; }

        public string? Description { get; set; }

        // Navigation
        public Account Account { get; set; } = null!;
    }

    public class Transaction
    {
        [NotMapped]
        public string? Id { get; set; }

        [Key]
        public Guid TransactionID { get; set; } = Guid.NewGuid();

        [Required]
        public Guid AccountID { get; set; }

        [Required, MaxLength(20)]
        public string TransactionType { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required, StringLength(3)]
        public string Currency { get; set; } = "";

        [Required]
        public DateTime TransactionDate { get; set; }

        public string? Description { get; set; }

        [MaxLength(50)]
        public string? ReferenceNumber { get; set; }

        // Navigation
        public Account Account { get; set; } = null!;
    }

    public class Portfolio
    {
        [NotMapped]
        public string? Id { get; set; }

        [Key]
        public Guid PortfolioID { get; set; } = Guid.NewGuid();

        [Required]
        public Guid CustomerID { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        public TblCustomer Customer { get; set; } = null!;
        public virtual ICollection<Investment> Investments { get; set; } = new List<Investment>();
    }

    public class Investment
    {
        [NotMapped]
        public string? Id { get; set; }

        [Key]
        public Guid InvestmentID { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PortfolioID { get; set; }

        [Required]
        public Guid SecurityID { get; set; }

        [Column(TypeName = "decimal(10,4)")]
        public decimal Quantity { get; set; }

        [Required]
        public DateOnly PurchaseDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PurchasePrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal CurrentValue { get; set; }

        // Navigation
        public Portfolio Portfolio { get; set; } = null!;
        public Security Security { get; set; } = null!;
    }

    public class Security
    {
        [NotMapped]
        public string? Id { get; set; }

        [Key]
        public Guid SecurityID { get; set; } = Guid.NewGuid();

        [Required, MaxLength(50)]
        public string SecurityType { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Symbol { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal MarketValue { get; set; }

        [Required, StringLength(3)]
        public string Currency { get; set; } = "";

        public DateOnly LastUpdated { get; set; }
    }

    public class Expense
    {
        [NotMapped]
        public string? Id { get; set; }

        [Key]
        public Guid ExpenseID { get; set; } = Guid.NewGuid();

        [Required]
        public Guid AccountID { get; set; }

        [Required, MaxLength(100)]
        public string Type { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateOnly ExpenseDate { get; set; }

        public string? Description { get; set; }

        // Navigation
        public Account Account { get; set; } = null!;
    }

    public class Loan
    {
        [NotMapped]
        public string? Id { get; set; }

        [Key]
        public Guid LoanID { get; set; } = Guid.NewGuid();

        [Required]
        public Guid CustomerID { get; set; }

        [Required, MaxLength(50)]
        public string LoanType { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal PrincipalAmount { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal InterestRate { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Balance { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Active";

        // Navigation
        public TblCustomer Customer { get; set; } = null!;
    }

    public class RecurringTransaction
    {
        [NotMapped]
        public string? Id { get; set; }

        [Key]
        public Guid RecurringTransactionID { get; set; } = Guid.NewGuid();

        [Required]
        public Guid AccountID { get; set; }

        [Required, MaxLength(20)]
        public string TransactionType { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required, StringLength(3)]
        public string Currency { get; set; } = "";

        [Required, MaxLength(20)]
        public string Frequency { get; set; } = string.Empty;

        [Required]
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public DateOnly? EndDate { get; set; }

        public DateOnly? NextExecutionDate { get; set; }

        public Account Account { get; set; } = null!;
    }
}
