using Microsoft.EntityFrameworkCore;
using PayrollService.Data;
using PayrollService.Models;

namespace PayrollService.Repositories;

public class CountryPolicyRepository : Repository<CountryPolicy>, ICountryPolicyRepository
{
    public CountryPolicyRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<CountryPolicy>> GetAllActiveAsync() =>
        await _db.CountryPolicies
            .Where(c => c.IsActive)
            .OrderBy(c => c.CountryName)
            .ToListAsync();

    public async Task<CountryPolicy?> GetByCountryCodeAsync(string countryCode) =>
        await _db.CountryPolicies
            .FirstOrDefaultAsync(c => c.CountryCode == countryCode && c.IsActive);

    public async Task<IEnumerable<TaxBracket>> GetTaxBracketsAsync(string countryCode) =>
        await _db.TaxBrackets
            .Where(t => t.Country == countryCode)
            .OrderBy(t => t.MinIncome)
            .ToListAsync();

    public async Task<IEnumerable<AgeBracket>> GetAgeBracketsAsync(string countryCode) =>
        await _db.AgeBrackets
            .Where(a => a.CountryCode == countryCode)
            .OrderBy(a => a.MinAge)
            .ToListAsync();

    public async Task<bool> CountryCodeExistsAsync(string countryCode) =>
        await _db.CountryPolicies
            .AnyAsync(c => c.CountryCode == countryCode);

    public async Task AddTaxBracketAsync(TaxBracket bracket) =>
        await _db.TaxBrackets.AddAsync(bracket);

    public async Task AddAgeBracketAsync(AgeBracket bracket) =>
        await _db.AgeBrackets.AddAsync(bracket);
}