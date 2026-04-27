using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Web.Controllers;

public class DevController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IPurchaseRecordService _purchaseRecordService;
    private readonly IUserService _userService;
    private readonly IMongoDatabase _db;

    public DevController(
        IOrderService orderService,
        IPurchaseRecordService purchaseRecordService,
        IUserService userService,
        IMongoDatabase db)
    {
        _orderService = orderService;
        _purchaseRecordService = purchaseRecordService;
        _userService = userService;
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> DeleteSeedOrders()
    {
        var collection = _db.GetCollection<BsonDocument>("Orders");
        var filter = Builders<BsonDocument>.Filter.Regex("orderNumber", new BsonRegularExpression("^SEED-"));
        var result = await collection.DeleteManyAsync(filter);
        return Content($"Deleted {result.DeletedCount} seed orders. PurchaseRecords were not touched.");
    }

    [HttpGet]
    public async Task<IActionResult> DeleteSeedPurchases()
    {
        var collection = _db.GetCollection<BsonDocument>("PurchaseRecords");
        var filter = Builders<BsonDocument>.Filter.Eq("referenceId", "seed");
        var result = await collection.DeleteManyAsync(filter);
        return Content($"Deleted {result.DeletedCount} seed purchase records.");
    }

    [HttpGet]
    public async Task<IActionResult> SeedStatisticsData()
    {
        var rng = new Random(99);
        var users = (await _userService.GetAllUsersAsync()).ToList();
        var userId = users.FirstOrDefault()?.Id ?? "000000000000000000000000";

        var hatNames = new[] { "Fedora", "Trilby", "Pork Pie", "Newsboy", "Bowler", "Panama" };
        var materialNames = new[] { "Ull", "Filt", "Bomull", "Siden", "Läder" };
        var itemNames = new[] { "Brodyr", "Bård", "Spänne", "Band", "Fjäder" };

        var now = DateTime.UtcNow;
        int ordersSeeded = 0;
        int purchasesSeeded = 0;

        for (int monthsBack = 35; monthsBack >= 0; monthsBack--)
        {
            var monthStart = new DateTime(now.Year, now.Month, 1).AddMonths(-monthsBack);
            var daysInMonth = DateTime.DaysInMonth(monthStart.Year, monthStart.Month);

            var isHighSeason = monthStart.Month >= 10 || monthStart.Month <= 1;
            var yearFactor = 1.0 + (monthStart.Year - 2023) * 0.10;

            var ordersThisMonth = rng.Next(isHighSeason ? 4 : 2, isHighSeason ? 7 : 5);
            var purchasesThisMonth = rng.Next(isHighSeason ? 6 : 4, isHighSeason ? 10 : 8);

            for (int o = 0; o < ordersThisMonth; o++)
            {
                var day = rng.Next(1, daysInMonth + 1);
                var orderDate = new DateTime(monthStart.Year, monthStart.Month, day, rng.Next(8, 18), rng.Next(0, 60), 0, DateTimeKind.Utc);
                var qty = rng.Next(1, 4);
                var unitPrice = (int)(rng.Next(900, 3800) * yearFactor);
                var isSpecial = rng.NextDouble() < 0.15;
                var isModified = !isSpecial && rng.NextDouble() < 0.30;

                var orderItem = new OrderItem
                {
                    ProductId = "seed",
                    Name = hatNames[rng.Next(hatNames.Length)],
                    Quantity = qty,
                    UnitPrice = unitPrice,
                    Size = new[] { "S", "M", "L", "XL" }[rng.Next(4)],
                    IsModified = isModified,
                    ModificationDescription = isModified ? "Seedat tillägg" : string.Empty,
                    SpecialHats = isSpecial ? new SpecialHats { Description = "Seedat special" } : null
                };

                var order = new Order
                {
                    OrderNumber = $"SEED-{orderDate:yyyyMMdd}-{o}",
                    OrderDate = orderDate,
                    UserId = userId,
                    Status = OrderStatus.Shipped,
                    TotalAmount = qty * unitPrice,
                    ShippingAddress = new ShippingAddress
                    {
                        FullName = "Seed Kund",
                        Address = "Seedgatan 1",
                        City = "Stockholm",
                        PostalCode = "11122",
                        Country = "Sverige",
                        CountryCode = "SE",
                        Phone = "0700000000"
                    },
                    Items = new List<OrderItem> { orderItem }
                };

                await _orderService.CreateOrderAsync(order);
                ordersSeeded++;
            }

            for (int p = 0; p < purchasesThisMonth; p++)
            {
                var day = rng.Next(1, daysInMonth + 1);
                var purchaseDate = new DateTime(monthStart.Year, monthStart.Month, day, rng.Next(8, 17), 0, 0, DateTimeKind.Utc);
                var useItem = rng.NextDouble() < 0.35;
                var name = useItem ? itemNames[rng.Next(itemNames.Length)] : materialNames[rng.Next(materialNames.Length)];
                var qty = rng.Next(5, 25);
                var unitCost = (decimal)(rng.Next(60, 450) * yearFactor);

                await _purchaseRecordService.CreateAsync(new PurchaseRecord
                {
                    Type = useItem ? "Item" : "Material",
                    ReferenceId = "seed",
                    Name = name,
                    Quantity = qty,
                    UnitCost = unitCost,
                    TotalCost = unitCost * qty,
                    PurchasedAt = purchaseDate
                });
                purchasesSeeded++;
            }
        }

        return Content($"Seeded {ordersSeeded} orders and {purchasesSeeded} purchase records across 3 years.");
    }

    [HttpGet]
    public async Task<IActionResult> Seed()
    {
        var rng = new Random(42);

        var users = (await _userService.GetAllUsersAsync()).ToList();
        var userId = users.FirstOrDefault()?.Id ?? "000000000000000000000000";

        var hatNames = new[] { "Fedora", "Trilby", "Pork Pie", "Newsboy", "Bowler", "Panama" };
        var materialNames = new[] { "Ull", "Filt", "Bomull", "Siden", "Läder" };
        var itemNames = new[] { "Brodyr", "Bård", "Spänne", "Band", "Fjäder" };

        var now = DateTime.UtcNow;
        int ordersSeeded = 0;
        int purchasesSeeded = 0;

        for (int daysBack = 730; daysBack >= 1; daysBack--)
        {
            var date = now.AddDays(-daysBack);

            var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
            var seasonBoost = (date.Month >= 10 || date.Month <= 1) ? 0.25 : 0.0;
            var orderChance = (isWeekend ? 0.30 : 0.60) + seasonBoost;
            if (rng.NextDouble() > orderChance) continue;

            int ordersThisDay = rng.Next(1, 5);
            for (int i = 0; i < ordersThisDay; i++)
            {
                var isSpecial = rng.NextDouble() < 0.15;
                var isModified = !isSpecial && rng.NextDouble() < 0.30;
                var qty = rng.Next(1, 4);
                var yearFactor = 1.0 + (date.Year - 2024) * 0.08;
                var unitPrice = (int)(rng.Next(800, 3500) * yearFactor);

                var orderItem = new OrderItem
                {
                    ProductId = "seed",
                    Name = hatNames[rng.Next(hatNames.Length)],
                    Quantity = qty,
                    UnitPrice = unitPrice,
                    Size = new[] { "S", "M", "L", "XL" }[rng.Next(4)],
                    IsModified = isModified,
                    ModificationDescription = isModified ? "Seedat tillägg" : string.Empty,
                    SpecialHats = isSpecial ? new SpecialHats { Description = "Seedat special" } : null
                };

                var order = new Order
                {
                    OrderNumber = $"SEED-{date:yyyyMMdd}-{i}",
                    OrderDate = date,
                    UserId = userId,
                    Status = OrderStatus.Shipped,
                    TotalAmount = qty * unitPrice,
                    ShippingAddress = new ShippingAddress
                    {
                        FullName = "Seed Kund",
                        Address = "Seedgatan 1",
                        City = "Stockholm",
                        PostalCode = "11122",
                        Country = "Sverige",
                        CountryCode = "SE",
                        Phone = "0700000000"
                    },
                    Items = new List<OrderItem> { orderItem }
                };

                await _orderService.CreateOrderAsync(order);
                ordersSeeded++;
            }

            if (rng.NextDouble() < 0.45)
            {
                var name = materialNames[rng.Next(materialNames.Length)];
                var useItem = rng.NextDouble() < 0.4;
                if (useItem) name = itemNames[rng.Next(itemNames.Length)];
                var qty = rng.Next(5, 30);
                var unitCost = (decimal)rng.Next(50, 400);

                await _purchaseRecordService.CreateAsync(new PurchaseRecord
                {
                    Type = useItem ? "Item" : "Material",
                    ReferenceId = "seed",
                    Name = name,
                    Quantity = qty,
                    UnitCost = unitCost,
                    TotalCost = unitCost * qty,
                    PurchasedAt = date
                });
                purchasesSeeded++;
            }
        }

        return Content($"Seeded {ordersSeeded} orders and {purchasesSeeded} purchase records. You can delete this endpoint when done.");
    }
}
