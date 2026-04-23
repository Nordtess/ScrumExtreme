using MongoDB.Driver;
using MongoDB.Bson;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Application.Services;
using ScrumExtreme.Domain.Interfaces;
using ScrumExtreme.Infrastructure.Repositories;
using ScrumExtreme.Web.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<AuthFilter>();
});

var mongoConnectionString = builder.Configuration["MongoDB:ConnectionString"]
    ?? throw new InvalidOperationException("MongoDB connection string not found.");
var mongoDatabaseName = builder.Configuration["MongoDB:DatabaseName"]
    ?? throw new InvalidOperationException("MongoDB database name not found.");

builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoConnectionString));
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDatabaseName));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IHatService, HatsService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<ICompanySettingsService, CompanySettingsService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseSession();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}")
    .WithStaticAssets();

try
{
    var client = app.Services.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase(mongoDatabaseName);
    await database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
    Console.WriteLine("✅ MongoDB connection successful!");

    var collectionNames = await (await database.ListCollectionNamesAsync()).ToListAsync();
    if (!collectionNames.Contains("Items"))
    {
        var validator = new BsonDocument
        {
            {
                "$jsonSchema", new BsonDocument
                {
                    { "bsonType", "object" },
                    { "required", new BsonArray { "name", "price", "stock" } },
                    {
                        "properties", new BsonDocument
                        {
                            {
                                "name", new BsonDocument
                                {
                                    { "bsonType", "string" },
                                    { "minLength", 1 },
                                    { "maxLength", 100 },
                                    { "description", "Namn på tillbehöret, obligatoriskt" }
                                }
                            },
                            {
                                "price", new BsonDocument
                                {
                                    { "bsonType", "double" },
                                    { "minimum", 0 },
                                    { "description", "Pris per enhet, måste vara >= 0" }
                                }
                            },
                            {
                                "stock", new BsonDocument
                                {
                                    { "bsonType", "int" },
                                    { "minimum", 0 },
                                    { "description", "Antal i lager, måste vara >= 0" }
                                }
                            }
                        }
                    }
                }
            }
        };
        var options = new CreateCollectionOptions<BsonDocument>
        {
            Validator = new BsonDocumentFilterDefinition<BsonDocument>(validator),
            ValidationLevel = DocumentValidationLevel.Strict,
            ValidationAction = DocumentValidationAction.Error
        };
        await database.CreateCollectionAsync("Items", options);
        Console.WriteLine("✅ Items collection created with schema validation.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ MongoDB connection FAILED: {ex.Message}");
}

app.Run();