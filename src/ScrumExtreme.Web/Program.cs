using MongoDB.Driver;
using MongoDB.Bson;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Application.Services;
using ScrumExtreme.Domain.Interfaces;
using ScrumExtreme.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Register MongoDB
var mongoConnectionString = builder.Configuration["MongoDB:ConnectionString"]
    ?? throw new InvalidOperationException("MongoDB connection string not found.");
var mongoDatabaseName = builder.Configuration["MongoDB:DatabaseName"]
    ?? throw new InvalidOperationException("MongoDB database name not found.");

builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoConnectionString));
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDatabaseName));

// Register generic repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Register application services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IHatService, HatsService>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=AddCustomer}/{action=Index}/{id?}")
    .WithStaticAssets();

// Ping MongoDB on startup to confirm connection
try
{
    var client = app.Services.GetRequiredService<IMongoClient>();
    await client.GetDatabase(mongoDatabaseName).RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
    Console.WriteLine("✅ MongoDB connection successful!");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ MongoDB connection FAILED: {ex.Message}");
}

app.Run();
