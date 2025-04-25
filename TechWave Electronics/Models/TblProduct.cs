using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechWave_Electronics.Models;

public partial class TblProduct
{
    [NotMapped]
    public string? Id { get; set; }
    [Key]
    public Guid ProductId { get; set; } = Guid.NewGuid();

    public Guid SupplierId { get; set; }

    public Guid CategoryId { get; set; }
    [StringLength(30)]
    public string ProductName { get; set; } = null!;
    [StringLength(30)]
    public string? EnglishName { get; set; }

    public int? QuantityPerUnit { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    [DataType(DataType.Currency)]
    public decimal UnitPrice { get; set; }
    public byte[]? itemImg { get; set; }
    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    public int UnitsInStock { get; set; }

    public int UnitsOnOrder { get; set; }

    public int ReorderLevel { get; set; }

    public bool Discontinued { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual TblSupplier Supplier { get; set; } = null!;

    public virtual ICollection<TblOrderDetail> TblOrderDetails { get; set; } = new List<TblOrderDetail>();
}
