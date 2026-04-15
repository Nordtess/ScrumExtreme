using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrumExtreme.Application.Services
{


    public class HatsService : IHatService
    {
        private readonly IRepository<Hats> _repository;

        public HatsService(IRepository<Hats> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Hats>> GetAllHatsAsync() =>
            await _repository.GetAllAsync();

        public async Task CreateHatsAsync(Hats hats) =>
            await _repository.AddAsync(hats);

        public async Task<Hats?> GetByIdAsync(string id) =>
            await _repository.GetByIdAsync(id);

        public async Task UpdateHatAsync(Hats hat) =>
            await _repository.UpdateAsync(hat.Id, hat);
    }


}
