using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Application.Interfaces;

public interface ICompanySettingsService
{
    Task<CompanySettings?> GetAsync();
    Task<decimal> GetCapitalAsync();
    Task AddCapitalAsync(decimal amount);
    Task DeductCapitalAsync(decimal amount);
}
