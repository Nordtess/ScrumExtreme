using System;
using System.Collections.Generic;
using System.Text;
using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Application.Interfaces
{
    public interface IMaterialService
    {
        Task<IEnumerable<Material>> GetMaterialsAsync();    
        Task CreateMaterialAsync(Material material);

    }
}
