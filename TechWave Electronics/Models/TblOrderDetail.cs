using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechWave_Electronics.Models;

public partial class TblOrderDetail
{
    [NotMapped]
    public string? Id { get; set; }
    [Key]
    public Guid OrderId { get; set; } = Guid.NewGuid();

    public Guid ProductId { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    [DataType(DataType.Currency)]
    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public float Discount { get; set; }

    public float Tax { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [DataType(DataType.Currency)]
    public decimal Total { get => CalculateGrandTotal(UnitPrice, Quantity, Discount, Tax); set { } } 


    public Guid OrderId1 { get; set; }

    public virtual TblOrder OrderId1Navigation { get; set; } = null!;

    public virtual TblProduct Product { get; set; } = null!;


    private static decimal CalculateGrandTotal(decimal unitPrice, int quantity, float discount, float tax)
    {
        decimal total = unitPrice * quantity;
        decimal discountAmount = total * (decimal)(discount / 100);
        decimal taxAmount = total * (decimal)(tax / 100);
        decimal grandTotal = total - discountAmount + taxAmount;

        return grandTotal;
    }

}
