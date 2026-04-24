using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Domain.Interfaces;

namespace ScrumExtreme.Application.Services;

public class CompanySettingsService : ICompanySettingsService
{
    private readonly IRepository<CompanySettings> _repository;

    public CompanySettingsService(IRepository<CompanySettings> repository)
    {
        _repository = repository;
    }

    public async Task<CompanySettings?> GetAsync()
    {
        var all = await _repository.GetAllAsync();
        return all.FirstOrDefault();
    }

    public async Task<decimal> GetCapitalAsync()
    {
        var settings = await GetAsync();
        return settings?.CapitalSEK ?? 0m;
    }

    public async Task AddCapitalAsync(decimal amount)
    {
        var settings = await GetAsync();
        if (settings == null) return;
        settings.CapitalSEK += amount;
        settings.LastUpdated = DateTime.UtcNow;
        await _repository.UpdateAsync(settings.Id, settings);
    }

    public async Task DeductCapitalAsync(decimal amount)
    {
        var settings = await GetAsync();
        if (settings == null) return;
        settings.CapitalSEK -= amount;
        settings.LastUpdated = DateTime.UtcNow;
        await _repository.UpdateAsync(settings.Id, settings);
    }
}
