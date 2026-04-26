namespace ScrumExtreme.Application.Interfaces;

public interface IStatisticsService
{
    Task<StatisticsResult> GetStatisticsAsync(string period);
}
