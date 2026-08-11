namespace EmployeeService.DTOs;

public class CreateEmployeeRequest
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public int? ManagerId { get; set; }
    public decimal BaseSalary { get; set; }
    public DateTime JoinDate { get; set; }
}

public class UpdateEmployeeRequest
{
    public string Phone { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public int? ManagerId { get; set; }
    public decimal BaseSalary { get; set; }
}

public class EmployeeResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public int? ManagerId { get; set; }
    public decimal BaseSalary { get; set; }
    public DateTime JoinDate { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ResignationDate { get; set; }
    public string Remarks { get; set; } = string.Empty;
}

public class DepartmentRequest
{
    public string Name { get; set; } = string.Empty;
}
public class DeactivateEmployeeRequest
{
    public string Status { get; set; } = string.Empty;
    public DateTime? ResignationDate { get; set; }
    public string Remarks { get; set; } = string.Empty;
}