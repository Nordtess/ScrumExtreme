using ScrumExtreme.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrumExtreme.Application.Interfaces
{
    public interface IHatService
    {
        Task<IEnumerable<Hats>> GetAllHatsAsync();
        Task CreateHatsAsync(Hats hats);
        Task<Hats?> GetByIdAsync(string id);
        Task UpdateHatAsync(Hats hat);
        Task DeleteHatAsync(string id);
    }
}
