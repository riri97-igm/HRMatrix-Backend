namespace EmployeeService.Models;

public class Department	
{
	public int Id { get; set; }
	public string Name { get; set; }
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
	public int DepartmentId { get; set; }
	public Department? Department { get; set; }
	public int? ManagerId { get; set; }
	public decimal BaseSalary { get; set; }
	public DateTime JoinDate { get; set; }
	public bool IsActive { get; set; } = true;
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


