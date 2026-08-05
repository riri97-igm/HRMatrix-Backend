namespace PayrollService.Services;

public interface ITaxService
{
    // Myanmar
    decimal CalculateMyanmarTax(decimal annualGrossSalary);
    decimal CalculateSocialSecurity(decimal basicSalary);

    // Singapore
    decimal CalculateCpfEmployee(decimal grossSalary, int age);
    decimal CalculateCpfEmployer(decimal grossSalary, int age);
}