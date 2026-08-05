namespace PayrollService.Services;

public class TaxService : ITaxService
{
    // Myanmar Progressive Income Tax 
    // Based on annual income in MMK
    public decimal CalculateMyanmarTax(decimal annualGrossSalary)
    {
        decimal tax = 0;

        if (annualGrossSalary <= 4800000)
        {
            // 0% bracket
            tax = 0;
        }
        else if (annualGrossSalary <= 10000000)
        {
            // 5% on amount above 4.8M
            tax = (annualGrossSalary - 4800000) * 0.05m;
        }
        else if (annualGrossSalary <= 20000000)
        {
            // 5% on 4.8M-10M + 10% on amount above 10M
            tax = (10000000 - 4800000) * 0.05m
                + (annualGrossSalary - 10000000) * 0.10m;
        }
        else if (annualGrossSalary <= 30000000)
        {
            // 5% + 10% + 15% on amount above 20M
            tax = (10000000 - 4800000) * 0.05m
                + (20000000 - 10000000) * 0.10m
                + (annualGrossSalary - 20000000) * 0.15m;
        }
        else
        {
            // 5% + 10% + 15% + 20% on amount above 30M
            tax = (10000000 - 4800000) * 0.05m
                + (20000000 - 10000000) * 0.10m
                + (30000000 - 20000000) * 0.15m
                + (annualGrossSalary - 30000000) * 0.20m;
        }

        // Return monthly tax (annual tax / 12)
        return Math.Round(tax / 12, 2);
    }

    // Myanmar Social Security 
    // Employee contributes 2% of basic salary
    public decimal CalculateSocialSecurity(decimal basicSalary)
    {
        return Math.Round(basicSalary * 0.02m, 2);
    }

    // Singapore CPF Employee Contribution 
    // Rate depends on age
    public decimal CalculateCpfEmployee(decimal grossSalary, int age)
    {
        var rate = age switch
        {
            <= 55 => 0.20m,   // 20%
            <= 60 => 0.15m,   // 15%
            <= 65 => 0.095m,  // 9.5%
            _ => 0.07m        // 7%
        };

        return Math.Round(grossSalary * rate, 2);
    }

    // Singapore CPF Employer Contribution
    // Employer also contributes — shown separately on payslip
    public decimal CalculateCpfEmployer(decimal grossSalary, int age)
    {
        var rate = age switch
        {
            <= 55 => 0.17m,   // 17%
            <= 60 => 0.13m,   // 13%
            <= 65 => 0.09m,   // 9%
            _ => 0.075m       // 7.5%
        };

        return Math.Round(grossSalary * rate, 2);
    }
}