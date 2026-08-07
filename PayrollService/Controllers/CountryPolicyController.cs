using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollService.DTOs;
using PayrollService.Models;
using PayrollService.Repositories;

namespace PayrollService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CountryPolicyController : ControllerBase
{
    private readonly ICountryPolicyRepository _countryRepo;

    public CountryPolicyController(ICountryPolicyRepository countryRepo)
    {
        _countryRepo = countryRepo;
    }

    // Get all active countries
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var countries = await _countryRepo.GetAllActiveAsync();
        var result = new List<CountryPolicyResponse>();

        foreach (var c in countries)
        {
            var taxBrackets = await _countryRepo.GetTaxBracketsAsync(c.CountryCode);
            var ageBrackets = await _countryRepo.GetAgeBracketsAsync(c.CountryCode);

            result.Add(new CountryPolicyResponse
            {
                Id = c.Id,
                CountryCode = c.CountryCode,
                CountryName = c.CountryName,
                Currency = c.Currency,
                FlagEmoji = c.FlagEmoji,
                SocialContributionLabel = c.SocialContributionLabel,
                SocialContributionEmployeeRate = c.SocialContributionEmployeeRate,
                SocialContributionEmployerRate = c.SocialContributionEmployerRate,
                HasProgressiveTax = c.HasProgressiveTax,
                HasAgeBased = c.HasAgeBased,
                IsActive = c.IsActive,
                TaxBrackets = taxBrackets.Select(t => new TaxBracketResponse
                {
                    Id = t.Id,
                    Country = t.Country,
                    MinIncome = t.MinIncome,
                    MaxIncome = t.MaxIncome,
                    TaxRate = t.TaxRate,
                    Description = t.Description
                }).ToList(),
                AgeBrackets = ageBrackets.Select(a => new AgeBracketResponse
                {
                    Id = a.Id,
                    CountryCode = a.CountryCode,
                    MinAge = a.MinAge,
                    MaxAge = a.MaxAge,
                    EmployeeRate = a.EmployeeRate,
                    EmployerRate = a.EmployerRate,
                    Description = a.Description
                }).ToList()
            });
        }

        return Ok(result);
    }

    // Get single country by code
    [HttpGet("{countryCode}")]
    public async Task<IActionResult> GetByCode(string countryCode)
    {
        var country = await _countryRepo.GetByCountryCodeAsync(countryCode);
        if (country == null) return NotFound();

        var taxBrackets = await _countryRepo.GetTaxBracketsAsync(countryCode);
        var ageBrackets = await _countryRepo.GetAgeBracketsAsync(countryCode);

        return Ok(new CountryPolicyResponse
        {
            Id = country.Id,
            CountryCode = country.CountryCode,
            CountryName = country.CountryName,
            Currency = country.Currency,
            FlagEmoji = country.FlagEmoji,
            SocialContributionLabel = country.SocialContributionLabel,
            SocialContributionEmployeeRate = country.SocialContributionEmployeeRate,
            SocialContributionEmployerRate = country.SocialContributionEmployerRate,
            HasProgressiveTax = country.HasProgressiveTax,
            HasAgeBased = country.HasAgeBased,
            IsActive = country.IsActive,
            TaxBrackets = taxBrackets.Select(t => new TaxBracketResponse
            {
                Id = t.Id,
                Country = t.Country,
                MinIncome = t.MinIncome,
                MaxIncome = t.MaxIncome,
                TaxRate = t.TaxRate,
                Description = t.Description
            }).ToList(),
            AgeBrackets = ageBrackets.Select(a => new AgeBracketResponse
            {
                Id = a.Id,
                CountryCode = a.CountryCode,
                MinAge = a.MinAge,
                MaxAge = a.MaxAge,
                EmployeeRate = a.EmployeeRate,
                EmployerRate = a.EmployerRate,
                Description = a.Description
            }).ToList()
        });
    }

    // Admin - Add new country
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCountryPolicyRequest request)
    {
        if (await _countryRepo.CountryCodeExistsAsync(request.CountryCode))
            return BadRequest(new { message = "Country code already exists" });

        var country = new CountryPolicy
        {
            CountryCode = request.CountryCode.ToUpper(),
            CountryName = request.CountryName,
            Currency = request.Currency,
            FlagEmoji = request.FlagEmoji,
            SocialContributionLabel = request.SocialContributionLabel,
            SocialContributionEmployeeRate = request.SocialContributionEmployeeRate,
            SocialContributionEmployerRate = request.SocialContributionEmployerRate,
            HasProgressiveTax = request.HasProgressiveTax,
            HasAgeBased = request.HasAgeBased,
            IsActive = true
        };

        await _countryRepo.AddAsync(country);
        await _countryRepo.SaveChangesAsync();
        return Ok(new { message = "Country policy created", id = country.Id });
    }

    // Admin - Add tax bracket to country
    [HttpPost("{countryCode}/tax-brackets")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddTaxBracket(
        string countryCode,
        [FromBody] CreateTaxBracketRequest request)
    {
        var country = await _countryRepo.GetByCountryCodeAsync(countryCode);
        if (country == null) return NotFound();

        var bracket = new TaxBracket
        {
            Country = countryCode.ToUpper(),
            MinIncome = request.MinIncome,
            MaxIncome = request.MaxIncome,
            TaxRate = request.TaxRate,
            Description = request.Description
        };

        await _countryRepo.AddTaxBracketAsync(bracket);
        await _countryRepo.SaveChangesAsync();
        return Ok(new { message = "Tax bracket added" });
    }

    // Admin - Add age bracket to country
    [HttpPost("{countryCode}/age-brackets")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddAgeBracket(
        string countryCode,
        [FromBody] CreateAgeBracketRequest request)
    {
        var country = await _countryRepo.GetByCountryCodeAsync(countryCode);
        if (country == null) return NotFound();

        var bracket = new AgeBracket
        {
            CountryCode = countryCode.ToUpper(),
            MinAge = request.MinAge,
            MaxAge = request.MaxAge,
            EmployeeRate = request.EmployeeRate,
            EmployerRate = request.EmployerRate,
            Description = request.Description
        };

        await _countryRepo.AddAgeBracketAsync(bracket);
        await _countryRepo.SaveChangesAsync();
        return Ok(new { message = "Age bracket added" });
    }

    // Admin - Deactivate country
    [HttpPut("{countryCode}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate(string countryCode)
    {
        var country = await _countryRepo.GetByCountryCodeAsync(countryCode);
        if (country == null) return NotFound();

        country.IsActive = false;
        _countryRepo.Update(country);
        await _countryRepo.SaveChangesAsync();
        return Ok(new { message = "Country deactivated" });
    }
}