using DocumentFormat.OpenXml.Bibliography;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechWave_Electronics.Models
{
    public class ITems
    {

        [NotMapped]
        public string? Id { get; set; }
        [Key]
        public Guid ItemId { get; set; } = Guid.NewGuid();
        public string ItemName { get; set; }
        public string Description { get; set; }
        public Guid CategoryID { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }
        public string ReoderLevel { get; set; }
        public Category Category { get; set; }
        public ICollection<History> History { get; set; } = new List<History>();
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public ICollection<StockOut> StockOut { get; set; } = new List<StockOut>();
        public ICollection<StockIn> StockIn { get; set; } = new List<StockIn>();
    }

    public class Warehouses
    {

        [NotMapped]
        public string? Id { get; set; }
        [Key]
        public Guid WarehouseId { get; set; } = Guid.NewGuid();
        public string WarehouseName { get; set; }
        public string Location { get; set; }
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public ICollection<StockOut> StockOut { get; set; } = new List<StockOut>();
        public ICollection<StockIn> StockIn { get; set; } = new List<StockIn>();
        public ICollection<History> History { get; set; } = new List<History>();

    }

    public class Inventory
    {

        [NotMapped]
        public string Id { get; set; }

        [Key]
        public Guid InventoryId { get; set; } = Guid.NewGuid();
        public Guid ItemId { get; set; }
        public Guid WarehouseId { get; set; }
        public int Quantity { get; set; }
        public Warehouses warehouses { get; set; }
        public ITems itms { get; set; }
    }

    public class StockIn
    {

        [NotMapped]
        public string? Id { get; set; }
        [Key]
        public Guid StockInId { get; set; } = Guid.NewGuid();
        public Guid ItemId { get; set; }
        public Guid WarehouseId { get; set; }
        public Guid SupplierId { get; set; }
        public int Quantity { get; set; }
        public DateOnly DateReceived { get; set; }
        public Warehouses warehouses { get; set; }
        public TblSupplier TblSupplier { get; set; }
        public ITems itms { get; set; }
    }

    public class StockOut
    {

        [NotMapped]
        public string? Id { get; set; }
        [Key]
        public Guid StockOutId { get; set; } = Guid.NewGuid();
        public Guid ItemId { get; set; }
        public Guid WarehouseId { get; set; }
        public int Quantity { get; set; }
        public DateOnly DateIssued { get; set; }
        public ITems itms { get; set; }
        public Warehouses warehouses { get; set; }
    }

    public class History
    {

        [NotMapped]
        public string? Id { get; set; }
        [Key]
        public Guid TransactionId { get; set; } = Guid.NewGuid();
        public Guid ItemId { get; set; }
        public Guid WarehouseId { get; set; }
        public int QuantityChange { get; set; }
        public string TransactionType { get; set; }
        public DateOnly Date { get; set; }
        public string Notes { get; set; }
        public ITems itms { get; set; }
        public Warehouses warehouses { get; set; }
    }
}
