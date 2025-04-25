using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechWave_Electronics.Models
{
    public partial class Employee
    {
        [NotMapped]
        public string? Id { get; set; }
        [Key]
        public Guid EmployeeId { get; set; } = Guid.NewGuid();
        public byte[]? Employeeurl { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; } 
        [StringLength(50)]
        public string? FirstName { get; set; }
        [StringLength(50)]
        public string? LastName { get; set; }
        public Guid JobID { get; set; }
        public DateOnly HireDate { get; set; }
        public DateOnly BirthDate { get; set; }
        [StringLength(60)]
        public string Address { get; set; } = null!;
        [StringLength(60)]
        public string City { get; set; } = null!;
        [StringLength(64)]
        public string Email { get; set; } = null!;
        [StringLength(14)]
        public string Phone { get; set; } = null!;
        [StringLength(50)]
        public string Country { get; set; } = null!;
        [Column(TypeName = "decimal(10,2)")]
        [DataType(DataType.Currency)]
        public decimal? Salary { get; set; }
        public int? Incentive { get; set; }
        public Guid? DepartmentsId { get; set; }
        public virtual Department? Departments { get; set; }
        public virtual Job? Job { get; set; }
        public virtual ICollection<Attendance> Attendance { get; set; } = new List<Attendance>();
        public virtual ICollection<TblOrder> TblOrders { get; set; } = new List<TblOrder>();
        public virtual ICollection<Leave> Leave { get; set; } = new List<Leave>();
        public virtual ICollection<Payroll> Payroll { get; set; } = new List<Payroll>();
    }

    public partial class Department
    {
        [NotMapped]
        public string? Id { get; set; }
        [Key]
        public Guid DepartmentsId { get; set; } = Guid.NewGuid();
        [StringLength(50)]
        public string? DepartmentName { get; set; }
        [StringLength(100)]
        public string? Location { get; set; }
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }

    public class Job
    {
        [NotMapped]
        public string? Id { get; set; }
        [Key]
        public Guid JobiD {  get; set; } = Guid.NewGuid();
        [StringLength(100)]
        public string? jobName { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        [DataType(DataType.Currency)]
        public decimal? BaseSalary { get; set; }
        public virtual ICollection<Employee> Employee { get; set; } = new List<Employee>();
    }

    public class Attendance
    {
        [NotMapped]
        public string? Id { get; set; }
        [Key]
        public Guid AttendanceID { get; set; } = Guid.NewGuid();
        public Guid EmployeeId { get; set; }
        public DateOnly AttendanceDate { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public virtual Employee? Employee { get; set; }
    }

    public class Payroll
    {
        [NotMapped]
        public string? Id { get; set; }
        [Key]
        public Guid PayrollID { get; set; } = Guid.NewGuid();
        public Guid EmployeeId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        [DataType(DataType.Currency)]
        public decimal? BaseSalary { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        [DataType(DataType.Currency)]
        public decimal? Deductions { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        [DataType(DataType.Currency)]
        public decimal? Allowances { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        [DataType(DataType.Currency)]
        public decimal? NetSalary { get; set; }
        public virtual Employee? Employee { get; set; }
    }

    public class Leave
    {
        [NotMapped]
        public string? Id { get; set; }
        [Key]
        public Guid LeaveID { get; set; } = Guid.NewGuid();
        public Guid EmployeeId { get; set; }
        [StringLength(50)]
        public string? LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [StringLength(20)]
        public string? Status { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}
