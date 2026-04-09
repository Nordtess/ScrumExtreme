using ScrumExtreme.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrumExtreme.Application.Interfaces
{
    internal interface IHatService
    {
        Task<IEnumerable<Hats>> GetAllHatsAsync();
        Task CreateUserAsync(Hats hats);
        Task<Hats?> GetByIdAsync(string id);
    }
}
