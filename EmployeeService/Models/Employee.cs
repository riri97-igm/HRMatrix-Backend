namespace EmployeeService.Models;

public enum EmployeeStatus
{
    Active,
    Resigned,
    Terminated,
    Retired,
    OnLeave
}

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}

public class Employee
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public Department? Department { get; set; }
    public int DepartmentId { get; set; }
    public int? ManagerId { get; set; }
    public decimal BaseSalary { get; set; }
    public DateTime JoinDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string Status { get; set; } = "Active";
    public DateTime? ResignationDate { get; set; }
    public string Remarks { get; set; } = string.Empty;

    // New Personal Info Fields
    public string Address { get; set; } = string.Empty;
    public DateTime? Birthday { get; set; }
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
    public string EmergencyContactRelation { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public class AuditLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string IpAddress { get; set; } = string.Empty;
}