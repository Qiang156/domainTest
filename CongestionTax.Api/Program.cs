using CongestionTax.Business.Extensions;
using CongestionTax.DataLayer.Extensions;
using CongestionTax.DataLayer.Repositories;
using CongestionTax.Domain;
using CongestionTax.Domain.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureAppConfiguration((hostContext, configurationBuilder) =>
    BuildConfiguration());

IConfiguration configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddDomainServices(configuration);
builder.Services.AddCongestionTax();
builder.Services.AddDataLayer();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
SeedRule();
var cancellationTokenSource = new CancellationTokenSource();
try
{
    await app.RunAsync(cancellationTokenSource.Token);
    await app.WaitForShutdownAsync(cancellationTokenSource.Token);
    await app.StopAsync(cancellationTokenSource.Token);
}
finally
{
    cancellationTokenSource.Cancel(false);
}

IConfiguration BuildConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();
    return builder.Build();
}

void SeedRule()
{
    var ruleRepo = app.Services.GetService<IRuleRepository>();

    ruleRepo.Upsert(new Rule("Gothenburg", new HashSet<TimeRange>
    {
        new TimeRange(TimeSpan.Parse("00:00"), 0),
        new TimeRange(TimeSpan.Parse("06:00"), 8),
        new TimeRange(TimeSpan.Parse("06:30"), 13),
        new TimeRange(TimeSpan.Parse("07:00"), 18),
        new TimeRange(TimeSpan.Parse("08:00"), 13),
        new TimeRange(TimeSpan.Parse("08:30"), 8),
        new TimeRange(TimeSpan.Parse("15:00"), 13),
        new TimeRange(TimeSpan.Parse("15:30"), 18),
        new TimeRange(TimeSpan.Parse("17:00"), 13),
        new TimeRange(TimeSpan.Parse("18:00"), 8),
        new TimeRange(TimeSpan.Parse("18:30"), 0)
    })
    {
        City = "Gothenburg",
        Version = 1,
        CountOnceWithinMinutes = 60,
        FreeForWeekend = true,
        FreeForHoliday = true,
        FreeBeforeHoliday = true,
        FreeMonth = 7,
        MaxAmountPerDay = 60,
        TollFreeVehicles = new List<Vehicle>
            { Vehicle.Motorcycle, Vehicle.Bus, Vehicle.Diplomat, Vehicle.Emergency, Vehicle.Foreign, Vehicle.Military }
    });

    // From
    // https://www.transportstyrelsen.se/en/road/road-tolls/Congestion-taxes-in-Stockholm-and-Goteborg/congestion-tax-in-stockholm/hours-and-amounts-in-stockholm/
    ruleRepo.Upsert(new Rule("Stockholm", new HashSet<TimeRange>
    {
        new TimeRange(TimeSpan.Parse("00:00"), 0),
        new TimeRange(TimeSpan.Parse("06:00"), 15),
        new TimeRange(TimeSpan.Parse("06:30"), 27),
        new TimeRange(TimeSpan.Parse("07:00"), 40),
        new TimeRange(TimeSpan.Parse("08:30"), 27),
        new TimeRange(TimeSpan.Parse("09:00"), 20),
        new TimeRange(TimeSpan.Parse("09:30"), 11),
        new TimeRange(TimeSpan.Parse("15:00"), 20),
        new TimeRange(TimeSpan.Parse("15:30"), 27),
        new TimeRange(TimeSpan.Parse("16:00"), 40),
        new TimeRange(TimeSpan.Parse("17:30"), 27),
        new TimeRange(TimeSpan.Parse("18:00"), 20),
        new TimeRange(TimeSpan.Parse("18:30"), 0)
    })
    {
        City = "Stockholm",
        Version = 1,
        CountOnceWithinMinutes = null,
        FreeForWeekend = true,
        FreeForHoliday = true,
        FreeBeforeHoliday = true,
        FreeMonth = 7,
        MaxAmountPerDay = 135,
        TollFreeVehicles = new List<Vehicle>
            { Vehicle.Motorcycle, Vehicle.Bus, Vehicle.Diplomat, Vehicle.Emergency, Vehicle.Foreign, Vehicle.Military }
    });
}

public partial class Program
{
}
