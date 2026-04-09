using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrumExtreme.Application.Services
{
    public class HatService
    {
        public class UserService : IHatService
        {
            private readonly IRepository<Hats> _repository;

            public UserService(IRepository<Hats> repository)
            {
                _repository = repository;
            }

            public async Task<IEnumerable<Hats>> GetAllHatsAsync() =>
                await _repository.GetAllAsync();

            public async Task CreateUserAsync(Hats hats) =>
                await _repository.AddAsync(hats);

            public async Task<Hats?> GetByIdAsync(string id) =>
                await _repository.GetByIdAsync(id);
        }

    }
}
