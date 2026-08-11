using PayrollService.Models;
using PayrollService.Repositories;

namespace PayrollService.Services;

public class TaxService : ITaxService
{
    private readonly ICountryPolicyRepository _countryRepo;

    public TaxService(ICountryPolicyRepository countryRepo)
    {
        _countryRepo = countryRepo;
    }

    // ── Progressive Tax Calculation ───────────────────────
    public async Task<decimal> CalculateTaxAsync(
        string countryCode, decimal annualGrossSalary)
    {
        var policy = await _countryRepo.GetByCountryCodeAsync(countryCode);
        if (policy == null || !policy.HasProgressiveTax) return 0;

        var brackets = await _countryRepo.GetTaxBracketsAsync(countryCode);
        decimal tax = 0;
        decimal remaining = annualGrossSalary;

        foreach (var bracket in brackets.OrderBy(b => b.MinIncome))
        {
            if (remaining <= 0) break;

            var bracketMin = bracket.MinIncome;
            var bracketMax = bracket.MaxIncome;
            var rate = bracket.TaxRate / 100;

            if (annualGrossSalary <= bracketMin) break;

            var taxableInThisBracket = Math.Min(
                annualGrossSalary - bracketMin,
                bracketMax - bracketMin
            );

            tax += taxableInThisBracket * rate;
            remaining -= taxableInThisBracket;
        }

        // Return monthly tax
        return Math.Round(tax / 12, 2);
    }

    // ── Social Contribution (SSB, EPF, SSS, SSF) ─────────
    public async Task<decimal> CalculateSocialContributionAsync(
        string countryCode, decimal basicSalary)
    {
        var policy = await _countryRepo.GetByCountryCodeAsync(countryCode);
        if (policy == null || policy.HasAgeBased) return 0;

        var rate = policy.SocialContributionEmployeeRate / 100;
        return Math.Round(basicSalary * rate, 2);
    }

    // ── Age Based Contribution (CPF Singapore) ────────────
    public async Task<(decimal Employee, decimal Employer)> CalculateAgeBasedContributionAsync(
        string countryCode, decimal grossSalary, int age)
    {
        var policy = await _countryRepo.GetByCountryCodeAsync(countryCode);
        if (policy == null || !policy.HasAgeBased) return (0, 0);

        var brackets = await _countryRepo.GetAgeBracketsAsync(countryCode);
        var bracket = brackets.FirstOrDefault(b => age >= b.MinAge && age <= b.MaxAge);
        if (bracket == null) return (0, 0);

        var employee = Math.Round(grossSalary * bracket.EmployeeRate / 100, 2);
        var employer = Math.Round(grossSalary * bracket.EmployerRate / 100, 2);

        return (employee, employer);
    }
}