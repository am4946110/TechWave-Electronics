using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechWave_Electronics.Models;

public partial class TblSupplier
{
    [NotMapped]
    public string? Id { get; set; }
    [Key]
    public Guid SupplierId { get; set; } = Guid.NewGuid();

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string ContactNumber { get; set; } = null!;

    [StringLength(50)]
    public string Address { get; set; } = null!;

    [StringLength(60)]
    public string City { get; set; } = null!;

    [StringLength(50)]
    public string Province { get; set; } = null!;

    public virtual ICollection<StockIn> StockIn { get; set; } = new List<StockIn>();

    public virtual ICollection<TblProduct> TblProducts { get; set; } = new List<TblProduct>();
}
