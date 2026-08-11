namespace PayrollService.Services;

public interface ITaxService
{
    Task<decimal> CalculateTaxAsync(string countryCode, decimal annualGrossSalary);
    Task<decimal> CalculateSocialContributionAsync(string countryCode, decimal basicSalary);
    Task<(decimal Employee, decimal Employer)> CalculateAgeBasedContributionAsync(
        string countryCode, decimal grossSalary, int age);
}