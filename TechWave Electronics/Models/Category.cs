using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechWave_Electronics.Models;

public partial class Category
{
    [NotMapped]
    public string? Id { get; set; }
    [Key]
    public Guid CategoryId { get; set; } = Guid.NewGuid();
    [StringLength(50)]
    public string? CategoryName { get; set; }

    public virtual ICollection<ITems> Tems { get; set; } = new List<ITems>();

    public virtual ICollection<TblProduct> TblProducts { get; set; } = new List<TblProduct>();
}
