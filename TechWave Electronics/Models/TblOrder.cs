using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechWave_Electronics.Models;

public partial class TblOrder
{
    [NotMapped]
    public string? Id { get; set; }
    [Key]
    public Guid OrderId { get; set; } = Guid.NewGuid();

    public Guid CustomerId { get; set; }

    public Guid EmployeeId { get; set; }
    [StringLength(7)]
    public string? Name { get; set; }

    [StringLength(50)]
    public string Address { get; set; } = null!;

    [StringLength(60)]
    public string City { get; set; } = null!;
    [StringLength(6)]
    public string Region { get; set; } = null!;
    [StringLength(7)]
    [DataType(DataType.PostalCode)]
    public string PostalCode { get; set; } = null!;
    [StringLength(50)]
    public string Country { get; set; } = null!;

    [StringLength(25)]
    [CreditCard(ErrorMessage = "Invalid credit card number")]
    public string ShipVia { get; set; } = null!;

    public DateOnly? OrderDate { get; set; }

    public DateOnly? RequiredDate { get; set; }

    public DateOnly? ShippedDate { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    [DataType(DataType.Currency)]
    public decimal? Freight { get; set; }

    public Guid? ShipViaNavigationShipperId { get; set; }

    public virtual TblCustomer Customer { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual TblShipper? ShipViaNavigationShipper { get; set; }

    public virtual ICollection<TblOrderDetail> TblOrderDetails { get; set; } = new List<TblOrderDetail>();
}
