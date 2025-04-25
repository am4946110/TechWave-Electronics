using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechWave_Electronics.Migrations.TechWaveElectronicsMigrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorys",
                columns: table => new
                {
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorys", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentsId);
                });

            migrationBuilder.CreateTable(
                name: "Job",
                columns: table => new
                {
                    JobiD = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    jobName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BaseSalary = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job", x => x.JobiD);
                });

            migrationBuilder.CreateTable(
                name: "TblCustomers",
                columns: table => new
                {
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactTitle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    City = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Region = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Active = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    Create_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerUrl = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblCustomers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "TblShippers",
                columns: table => new
                {
                    ShipperId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblShippers", x => x.ShipperId);
                });

            migrationBuilder.CreateTable(
                name: "TblSuppliers",
                columns: table => new
                {
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Province = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblSuppliers", x => x.SupplierId);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.WarehouseId);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ReoderLevel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_Items_Categorys_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categorys",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Employeeurl = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    JobID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HireDate = table.Column<DateOnly>(type: "date", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    City = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Incentive = table.Column<int>(type: "int", nullable: true),
                    DepartmentsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentsId",
                        column: x => x.DepartmentsId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentsId");
                    table.ForeignKey(
                        name: "FK_Employees_Job_JobID",
                        column: x => x.JobID,
                        principalTable: "Job",
                        principalColumn: "JobiD",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblProducts",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EnglishName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    QuantityPerUnit = table.Column<int>(type: "int", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    itemImg = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    UnitsInStock = table.Column<int>(type: "int", nullable: false),
                    UnitsOnOrder = table.Column<int>(type: "int", nullable: false),
                    ReorderLevel = table.Column<int>(type: "int", nullable: false),
                    Discontinued = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblProducts", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_TblProducts_Categorys_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categorys",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblProducts_TblSuppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "TblSuppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuantityChange = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    itmsItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    warehousesWarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_History_Items_itmsItemId",
                        column: x => x.itmsItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_History_Warehouses_warehousesWarehouseId",
                        column: x => x.warehousesWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    InventoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    warehousesWarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    itmsItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.InventoryId);
                    table.ForeignKey(
                        name: "FK_Inventory_Items_itmsItemId",
                        column: x => x.itmsItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inventory_Warehouses_warehousesWarehouseId",
                        column: x => x.warehousesWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockIn",
                columns: table => new
                {
                    StockInId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    DateReceived = table.Column<DateOnly>(type: "date", nullable: false),
                    warehousesWarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TblSupplierSupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    itmsItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockIn", x => x.StockInId);
                    table.ForeignKey(
                        name: "FK_StockIn_Items_itmsItemId",
                        column: x => x.itmsItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockIn_TblSuppliers_TblSupplierSupplierId",
                        column: x => x.TblSupplierSupplierId,
                        principalTable: "TblSuppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockIn_Warehouses_warehousesWarehouseId",
                        column: x => x.warehousesWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockOut",
                columns: table => new
                {
                    StockOutId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    DateIssued = table.Column<DateOnly>(type: "date", nullable: false),
                    itmsItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    warehousesWarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockOut", x => x.StockOutId);
                    table.ForeignKey(
                        name: "FK_StockOut_Items_itmsItemId",
                        column: x => x.itmsItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockOut_Warehouses_warehousesWarehouseId",
                        column: x => x.warehousesWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attendance",
                columns: table => new
                {
                    AttendanceID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CheckIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOut = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendance", x => x.AttendanceID);
                    table.ForeignKey(
                        name: "FK_Attendance_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "leaves",
                columns: table => new
                {
                    LeaveID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaveType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leaves", x => x.LeaveID);
                    table.ForeignKey(
                        name: "FK_leaves_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payrolls",
                columns: table => new
                {
                    PayrollID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    BaseSalary = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Deductions = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Allowances = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    NetSalary = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payrolls", x => x.PayrollID);
                    table.ForeignKey(
                        name: "FK_payrolls_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblOrders",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShipVia = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    OrderDate = table.Column<DateOnly>(type: "date", nullable: true),
                    RequiredDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ShippedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Freight = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ShipViaNavigationShipperId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblOrders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_TblOrders_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblOrders_TblCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "TblCustomers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblOrders_TblShippers_ShipViaNavigationShipperId",
                        column: x => x.ShipViaNavigationShipperId,
                        principalTable: "TblShippers",
                        principalColumn: "ShipperId");
                });

            migrationBuilder.CreateTable(
                name: "TblOrderDetails",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Discount = table.Column<float>(type: "real", nullable: false),
                    Tax = table.Column<float>(type: "real", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    OrderId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId1NavigationOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblOrderDetails", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_TblOrderDetails_TblOrders_OrderId1NavigationOrderId",
                        column: x => x.OrderId1NavigationOrderId,
                        principalTable: "TblOrders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblOrderDetails_TblProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "TblProducts",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_EmployeeId",
                table: "Attendance",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentsId",
                table: "Employees",
                column: "DepartmentsId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_JobID",
                table: "Employees",
                column: "JobID");

            migrationBuilder.CreateIndex(
                name: "IX_History_itmsItemId",
                table: "History",
                column: "itmsItemId");

            migrationBuilder.CreateIndex(
                name: "IX_History_warehousesWarehouseId",
                table: "History",
                column: "warehousesWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_itmsItemId",
                table: "Inventory",
                column: "itmsItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_warehousesWarehouseId",
                table: "Inventory",
                column: "warehousesWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryID",
                table: "Items",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_leaves_EmployeeId",
                table: "leaves",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_payrolls_EmployeeId",
                table: "payrolls",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockIn_itmsItemId",
                table: "StockIn",
                column: "itmsItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockIn_TblSupplierSupplierId",
                table: "StockIn",
                column: "TblSupplierSupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_StockIn_warehousesWarehouseId",
                table: "StockIn",
                column: "warehousesWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOut_itmsItemId",
                table: "StockOut",
                column: "itmsItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOut_warehousesWarehouseId",
                table: "StockOut",
                column: "warehousesWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_TblOrderDetails_OrderId1NavigationOrderId",
                table: "TblOrderDetails",
                column: "OrderId1NavigationOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TblOrderDetails_ProductId",
                table: "TblOrderDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_TblOrders_CustomerId",
                table: "TblOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TblOrders_EmployeeId",
                table: "TblOrders",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TblOrders_ShipViaNavigationShipperId",
                table: "TblOrders",
                column: "ShipViaNavigationShipperId");

            migrationBuilder.CreateIndex(
                name: "IX_TblProducts_CategoryId",
                table: "TblProducts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TblProducts_SupplierId",
                table: "TblProducts",
                column: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendance");

            migrationBuilder.DropTable(
                name: "History");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "leaves");

            migrationBuilder.DropTable(
                name: "payrolls");

            migrationBuilder.DropTable(
                name: "StockIn");

            migrationBuilder.DropTable(
                name: "StockOut");

            migrationBuilder.DropTable(
                name: "TblOrderDetails");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "TblOrders");

            migrationBuilder.DropTable(
                name: "TblProducts");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "TblCustomers");

            migrationBuilder.DropTable(
                name: "TblShippers");

            migrationBuilder.DropTable(
                name: "Categorys");

            migrationBuilder.DropTable(
                name: "TblSuppliers");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Job");
        }
    }
}
