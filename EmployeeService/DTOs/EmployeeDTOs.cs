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
    public string Address { get; set; } = string.Empty;
    public DateTime? Birthday { get; set; }
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
    public string EmergencyContactRelation { get; set; } = string.Empty;
}

public class UpdateEmployeeRequest
{
    public string Phone { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public int? ManagerId { get; set; }
    public decimal BaseSalary { get; set; }
    public string Address { get; set; } = string.Empty;
    public DateTime? Birthday { get; set; }
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
    public string EmergencyContactRelation { get; set; } = string.Empty;
}

public class UpdatePersonalInfoRequest
{
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime? Birthday { get; set; }
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
    public string EmergencyContactRelation { get; set; } = string.Empty;
}

public class EmployeeResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int? ManagerId { get; set; }
    public string ManagerName { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; }
    public DateTime JoinDate { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ResignationDate { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime? Birthday { get; set; }
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
    public string EmergencyContactRelation { get; set; } = string.Empty;
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