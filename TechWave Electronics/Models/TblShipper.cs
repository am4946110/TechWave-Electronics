using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechWave_Electronics.Models;

public partial class TblShipper
{
    [NotMapped]
    public string? Id { get; set; }
    [Key]
    public Guid ShipperId { get; set; } = Guid.NewGuid();

    [StringLength(50)]
    public string CompanyName { get; set; } = null!;

    public virtual ICollection<TblOrder> TblOrders { get; set; } = new List<TblOrder>();
}
