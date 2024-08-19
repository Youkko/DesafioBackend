using DatabaseConfig = MotorcycleRental.Models.Database.DatabaseConfig;
using MotorcycleService;
using MotorcycleRental.Data;
using MotorcycleRental.Models;
using DeliveryPersonService;

var builder = Host.CreateApplicationBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MotorcycleRentalDatabase");

builder.Services.Configure<DatabaseConfig>(o => o.ConnectionString = connectionString);
builder.Services.AddTransient<IDatabase, Database>();
builder.Services.AddTransient<IVehicleOps, VehicleOps>();
builder.Services.Configure<RabbitMQSettings>(options =>
{
    options.HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
    options.UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER");
    options.Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS");
});

builder.Services.AddHostedService<MessengerService>();

var host = builder.Build();
host.Run();