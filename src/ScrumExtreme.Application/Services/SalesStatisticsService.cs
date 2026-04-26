using ScrumExtreme.Application.Interfaces;

namespace ScrumExtreme.Domain.Entities;

public class SalesStatisticsService : ISalesStatisticsService
{
    public Task UpdateStatisticsAsync(decimal orderAmount, DateTime orderDate) =>
        Task.CompletedTask;
}
