using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechWave_Electronics.Models;

public partial class TblCustomer
{
    [NotMapped]
    public string? Id { get; set; }
    [Key]
    public Guid CustomerId { get; set; } = Guid.NewGuid();
    [StringLength(15)]
    public string? CompanyName { get; set; } 
    [StringLength(50)]
    public string? FirstName { get; set; }
    [StringLength(50)]
    public string? LastName { get; set; }
    [StringLength(50)]
    public string? ContactTitle { get; set; }
    [StringLength (60)]
    public string? Address { get; set; }
    [StringLength(60)]
    public string? City { get; set; }
    [StringLength(50)]
    public string? Region { get; set; }
    [StringLength(7)]
    [DataType(DataType.PostalCode)]
    public string? PostalCode { get; set; }
    [StringLength(50)]
    public string? Country { get; set; }
    [StringLength(14)]
    public string? Phone { get; set; }
    [StringLength(64)]
    public string? Email { get; set; }
    [StringLength(10)]
    public string? Fax { get; set; }
    public char? Active { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; } = DateTime.Now;

    public byte[]? CustomerUrl { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    public virtual ICollection<TblOrder> TblOrders { get; set; } = new List<TblOrder>();

    public virtual ICollection<Account> Accoount { get; set; } = new List<Account>();
    public virtual ICollection<Loan> Loan { get; set; } = new List<Loan>();
}
