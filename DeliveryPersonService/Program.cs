using DatabaseConfig = MotorcycleRental.Models.Database.DatabaseConfig;
using DeliveryPersonService;
using MotorcycleRental.Data;
using MotorcycleRental.Models;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MotorcycleRentalDatabase");

builder.Services.Configure<DatabaseConfig>(o => o.ConnectionString = connectionString);
builder.Services.AddTransient<IDatabase, Database>();
builder.Services.AddTransient<IAuthentication, Authentication>();
builder.Services.Configure<RabbitMQSettings>(options =>
{
    options.HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
    options.UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER");
    options.Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS");
});

builder.Services.AddHostedService<MessengerService>();

var host = builder.Build();

// Trigger database migration
using (var scope = host.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<IDatabase>();
    database.Migrate();
}

host.Run();