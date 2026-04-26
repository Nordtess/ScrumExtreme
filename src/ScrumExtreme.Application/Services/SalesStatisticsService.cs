using MongoDB.Driver;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Domain.Entities;
public class SalesStatisticsService : ISalesStatisticsService
{
    private readonly IMongoCollection<SalesStatistic> _salesStatisticCollection;

    public SalesStatisticsService(IMongoDatabase database)
    {
        _salesStatisticCollection = database.GetCollection<SalesStatistic>("Statistics");
    }

    public async Task UpdateStatisticsAsync(decimal orderAmount, DateTime orderDate)
    {
        var year = orderDate.Year;
        var month = orderDate.Month;
        var quarter = ((month - 1) / 3) + 1;
        var now = DateTime.UtcNow;

        await UpdatePeriodAsync("MONTH", year, month, quarter, orderAmount, now);
        await UpdatePeriodAsync("QUARTER", year, null, quarter, orderAmount, now);
        await UpdatePeriodAsync("YEAR", year, null, null, orderAmount, now);
    }

    private async Task UpdatePeriodAsync(
        string periodType,
        int year,
        int? month,
        int? quarter,
        decimal amount,
        DateTime now)
    {
        var filter = Builders<SalesStatistic>.Filter.Eq(x => x.PeriodType, periodType)
                     & Builders<SalesStatistic>.Filter.Eq(x => x.Year, year);

        if (month.HasValue)
            filter &= Builders<SalesStatistic>.Filter.Eq(x => x.Month, month.Value);

        if (quarter.HasValue)
            filter &= Builders<SalesStatistic>.Filter.Eq(x => x.Quarter, quarter.Value);

        var update = Builders<SalesStatistic>.Update
            .Inc(x => x.TotalSales, amount)
            .Inc(x => x.TotalOrders, 1)
            .Set(x => x.UpdatedAt, now)
            .SetOnInsert(x => x.CreatedAt, now)
            .SetOnInsert(x => x.PeriodType, periodType)
            .SetOnInsert(x => x.Year, year)
            .SetOnInsert(x => x.Month, month)
            .SetOnInsert(x => x.Quarter, quarter);

        await _salesStatisticCollection.UpdateOneAsync(
            filter,
            update,
            new UpdateOptions { IsUpsert = true }
        );
    }
}