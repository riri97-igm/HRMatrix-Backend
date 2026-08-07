using PayrollService.Models;

namespace PayrollService.Repositories;

public interface ICountryPolicyRepository : IRepository<CountryPolicy>
{
    Task<IEnumerable<CountryPolicy>> GetAllActiveAsync();
    Task<CountryPolicy?> GetByCountryCodeAsync(string countryCode);
    Task<IEnumerable<TaxBracket>> GetTaxBracketsAsync(string countryCode);
    Task<IEnumerable<AgeBracket>> GetAgeBracketsAsync(string countryCode);
    Task<bool> CountryCodeExistsAsync(string countryCode);
    Task AddTaxBracketAsync(TaxBracket bracket);
    Task AddAgeBracketAsync(AgeBracket bracket);
}