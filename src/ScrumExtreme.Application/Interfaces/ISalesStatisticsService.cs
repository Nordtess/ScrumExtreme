namespace ScrumExtreme.Application.Interfaces;

public interface ISalesStatisticsService
{
    Task UpdateStatisticsAsync(decimal orderAmount, DateTime orderDate);
}