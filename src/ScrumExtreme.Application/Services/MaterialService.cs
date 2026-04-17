using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Domain.Interfaces;

namespace ScrumExtreme.Application.Services;

public class MaterialService : IMaterialService
{
    private readonly IRepository<Material> _repository;

    public MaterialService(IRepository<Material> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Material>> GetMaterialsAsync() =>
        await _repository.GetAllAsync();

    public async Task CreateMaterialAsync(Material material) =>
        await _repository.AddAsync(material);
}