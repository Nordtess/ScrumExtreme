using System.Globalization;
using System.Text.Json;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Domain.Interfaces;

namespace ScrumExtreme.Application.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<PurchaseRecord> _purchaseRepository;
    private static readonly CultureInfo SwedishCulture = new("sv-SE");

    public StatisticsService(
        IRepository<Order> orderRepository,
        IRepository<PurchaseRecord> purchaseRepository)
    {
        _orderRepository = orderRepository;
        _purchaseRepository = purchaseRepository;
    }

    public async Task<StatisticsResult> GetStatisticsAsync(string period)
    {
        var orders = (await _orderRepository.GetAllAsync()).ToList();
        var purchases = (await _purchaseRepository.GetAllAsync()).ToList();

        var (labels, revenues, costs, orderCounts) = period switch
        {
            "week" => ComputeWeekly(orders, purchases),
            "quarter" => ComputeQuarterly(orders, purchases),
            "year" => ComputeYearly(orders, purchases),
            _ => ComputeMonthly(orders, purchases)
        };

        var result = new StatisticsResult
        {
            Period = period,
            ChartLabels = JsonSerializer.Serialize(labels),
            RevenueData = JsonSerializer.Serialize(revenues),
            CostData = JsonSerializer.Serialize(costs),
            OrderCountData = JsonSerializer.Serialize(orderCounts),
            TotalRevenue = revenues.Sum(),
            TotalCosts = costs.Sum(),
            TotalOrders = orderCounts.Sum()
        };

        result.NetProfit = result.TotalRevenue - result.TotalCosts;

        result.SpecialOrders = orders.Count(o => o.Items != null && o.Items.Any(i => i.SpecialHats != null));
        result.ModifiedOrders = orders.Count(o => o.Items != null
            && o.Items.Any(i => i.IsModified)
            && !o.Items.Any(i => i.SpecialHats != null));
        result.StandardOrders = orders.Count - result.SpecialOrders - result.ModifiedOrders;

        return result;
    }

    private static (List<string>, List<decimal>, List<decimal>, List<int>) ComputeMonthly(
        List<Order> orders, List<PurchaseRecord> purchases)
    {
        var labels = new List<string>();
        var revenues = new List<decimal>();
        var costs = new List<decimal>();
        var orderCounts = new List<int>();
        var now = DateTime.UtcNow;

        for (int i = 11; i >= 0; i--)
        {
            var d = now.AddMonths(-i);
            labels.Add(d.ToString("MMM yyyy", SwedishCulture));
            revenues.Add((decimal)orders
                .Where(o => o.OrderDate.Year == d.Year && o.OrderDate.Month == d.Month)
                .Sum(o => o.TotalAmount));
            costs.Add(purchases
                .Where(p => p.PurchasedAt.Year == d.Year && p.PurchasedAt.Month == d.Month)
                .Sum(p => p.TotalCost));
            orderCounts.Add(orders.Count(o => o.OrderDate.Year == d.Year && o.OrderDate.Month == d.Month));
        }

        return (labels, revenues, costs, orderCounts);
    }

    private static (List<string>, List<decimal>, List<decimal>, List<int>) ComputeWeekly(
        List<Order> orders, List<PurchaseRecord> purchases)
    {
        var labels = new List<string>();
        var revenues = new List<decimal>();
        var costs = new List<decimal>();
        var orderCounts = new List<int>();
        var now = DateTime.UtcNow.Date;

        int daysToMonday = ((int)now.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var thisWeekStart = now.AddDays(-daysToMonday);

        for (int i = 11; i >= 0; i--)
        {
            var weekStart = thisWeekStart.AddDays(-(7 * i));
            var weekEnd = weekStart.AddDays(7);
            int weekNum = ISOWeek.GetWeekOfYear(weekStart);
            labels.Add($"V.{weekNum} {weekStart.Year}");
            revenues.Add((decimal)orders
                .Where(o => o.OrderDate >= weekStart && o.OrderDate < weekEnd)
                .Sum(o => o.TotalAmount));
            costs.Add(purchases
                .Where(p => p.PurchasedAt >= weekStart && p.PurchasedAt < weekEnd)
                .Sum(p => p.TotalCost));
            orderCounts.Add(orders.Count(o => o.OrderDate >= weekStart && o.OrderDate < weekEnd));
        }

        return (labels, revenues, costs, orderCounts);
    }

    private static (List<string>, List<decimal>, List<decimal>, List<int>) ComputeQuarterly(
        List<Order> orders, List<PurchaseRecord> purchases)
    {
        var labels = new List<string>();
        var revenues = new List<decimal>();
        var costs = new List<decimal>();
        var orderCounts = new List<int>();
        var now = DateTime.UtcNow;
        int currentQuarter = (now.Month - 1) / 3 + 1;

        for (int i = 7; i >= 0; i--)
        {
            int totalQ = (now.Year * 4 + currentQuarter - 1) - i;
            int year = totalQ / 4;
            int quarter = (totalQ % 4) + 1;
            int startMonth = (quarter - 1) * 3 + 1;
            int endMonth = startMonth + 3;

            labels.Add($"Q{quarter} {year}");
            revenues.Add((decimal)orders
                .Where(o => o.OrderDate.Year == year && o.OrderDate.Month >= startMonth && o.OrderDate.Month < endMonth)
                .Sum(o => o.TotalAmount));
            costs.Add(purchases
                .Where(p => p.PurchasedAt.Year == year && p.PurchasedAt.Month >= startMonth && p.PurchasedAt.Month < endMonth)
                .Sum(p => p.TotalCost));
            orderCounts.Add(orders.Count(o => o.OrderDate.Year == year && o.OrderDate.Month >= startMonth && o.OrderDate.Month < endMonth));
        }

        return (labels, revenues, costs, orderCounts);
    }

    private static (List<string>, List<decimal>, List<decimal>, List<int>) ComputeYearly(
        List<Order> orders, List<PurchaseRecord> purchases)
    {
        var labels = new List<string>();
        var revenues = new List<decimal>();
        var costs = new List<decimal>();
        var orderCounts = new List<int>();
        var now = DateTime.UtcNow;

        for (int i = 4; i >= 0; i--)
        {
            int year = now.Year - i;
            labels.Add(year.ToString());
            revenues.Add((decimal)orders
                .Where(o => o.OrderDate.Year == year)
                .Sum(o => o.TotalAmount));
            costs.Add(purchases
                .Where(p => p.PurchasedAt.Year == year)
                .Sum(p => p.TotalCost));
            orderCounts.Add(orders.Count(o => o.OrderDate.Year == year));
        }

        return (labels, revenues, costs, orderCounts);
    }
}
