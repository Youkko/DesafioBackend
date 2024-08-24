using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using MotorcycleRental.Models.Database;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
namespace MotorcycleRental.Data
{
    public class DatabaseContext : DbContext
    {
        private readonly string _connectionString;

        #region DBSets
        public DbSet<Delivery>? Delivery { get; set; }
        public DbSet<DeliveryPerson>? DeliveryPerson { get; set; }
        public DbSet<Vehicle>? Vehicle { get; set; }
        public DbSet<Order>? Order { get; set; }
        public DbSet<OrderItem>? OrderItem { get; set; }
        public DbSet<Rental>? Rental { get; set; }
        public DbSet<RentalPlan>? RentalPlan { get; set; }
        public DbSet<User>? User { get; set; }
        public DbSet<UserType>? UserType { get; set; }
        public DbSet<CNHType>? CNHType { get; set; }
        public DbSet<Notification>? Notification { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(_connectionString);

        public DatabaseContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Database connection context
        /// </summary>
        /// <param name="connectionString">Sample QueryString: "Host=my_host;Database=my_db;Username=my_user;Password=my_pw"</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Irrelevant for this project")]
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            _connectionString = options
                .FindExtension<NpgsqlOptionsExtension>()?
                .ConnectionString ??
                string.Empty;
        }

        #region Model creation and initial data seed
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SetupCNHTypes(modelBuilder);
            SetupRentalPlans(modelBuilder);
            SetupRentals(modelBuilder);
            SetupDeliveryPerson(modelBuilder);
            SetupUser(modelBuilder, SetupUserType(modelBuilder));
            SetupVehicle(modelBuilder);
            SetupDelivery(modelBuilder);

            SetupDefaultFields<Order>(modelBuilder);
            SetupDefaultFields<OrderItem>(modelBuilder);
            SetupDefaultFields<Notification>(modelBuilder);
        }

        private void SetupDefaultFields<T>(ModelBuilder modelBuilder)
            where T : ModelBase
        {
            var b = modelBuilder.Entity<T>();
            b.Property(p => p.Id).HasDefaultValueSql("gen_random_uuid()");
            b.Property(p => p.CreatedOn).HasDefaultValueSql("now()");
        }

        private void SetupRentalPlans(ModelBuilder modelBuilder)
        {
            var RentalPlans = new List<RentalPlan>
            {
                new RentalPlan()
                {
                    Id = Guid.NewGuid(),
                    Days = 7,
                    Value = 30.0,
                    CreatedOn = DateTime.UtcNow
                },
                new RentalPlan()
                {
                    Id = Guid.NewGuid(),
                    Days = 15,
                    Value = 28.0,
                    CreatedOn = DateTime.UtcNow
                },
                new RentalPlan()
                {
                    Id = Guid.NewGuid(),
                    Days = 30,
                    Value = 22.0,
                    CreatedOn = DateTime.UtcNow
                },
                new RentalPlan()
                {
                    Id = Guid.NewGuid(),
                    Days = 45,
                    Value = 20.0,
                    CreatedOn = DateTime.UtcNow
                },
                new RentalPlan()
                {
                    Id = Guid.NewGuid(),
                    Days = 50,
                    Value = 18.0,
                    CreatedOn = DateTime.UtcNow
                }
            };
            SetupDefaultFields<RentalPlan>(modelBuilder);
            var b = modelBuilder.Entity<RentalPlan>();
            b.HasIndex(p => p.Days).IsUnique();
            b.Property(p => p.Days).IsRequired();
            b.Property(p => p.Value).IsRequired();
            b.HasData(RentalPlans);
        }

        private void SetupRentals(ModelBuilder modelBuilder)
        {
            SetupDefaultFields<Rental>(modelBuilder);
            var b = modelBuilder.Entity<Rental>();
            b.Property(p => p.StartDate).IsRequired();
            b.Property(p => p.EndDate).IsRequired();

            b.HasOne(r => r.User)
             .WithMany(u => u.Rentals)
             .HasForeignKey(dp => dp.UserId)
             .IsRequired();
            
            b.HasOne(r => r.Vehicle)
             .WithMany(v => v.Rentals)
             .HasForeignKey(r => r.VehicleId)
             .IsRequired();

            b.HasOne(r => r.RentalPlan)
             .WithMany(v => v.Rentals)
             .HasForeignKey(r => r.RentalPlanId)
             .IsRequired();
        }

        private void SetupCNHTypes(ModelBuilder modelBuilder)
        {
            var CNHTypes = new List<CNHType>
            {
                new CNHType()
                {
                    Id = Guid.NewGuid(),
                    Type = "A",
                    CreatedOn = DateTime.UtcNow
                },
                new CNHType()
                {
                    Id = Guid.NewGuid(),
                    Type = "B",
                    CreatedOn = DateTime.UtcNow
                },
                new CNHType()
                {
                    Id = Guid.NewGuid(),
                    Type = "AB",
                    CreatedOn = DateTime.UtcNow
                }
            };
            SetupDefaultFields<CNHType>(modelBuilder);
            var b = modelBuilder.Entity<CNHType>();
            b.HasIndex(p => p.Type).IsUnique();
            b.Property(p => p.Type).IsRequired();
            b.HasData(CNHTypes);
        }

        private void SetupDeliveryPerson(ModelBuilder modelBuilder)
        {
            SetupDefaultFields<DeliveryPerson>(modelBuilder);
            var b = modelBuilder.Entity<DeliveryPerson>();
            b.HasIndex(p => p.CNPJ).IsUnique();
            b.HasIndex(p => p.CNH).IsUnique();
            b.Property(p => p.CNPJ).IsRequired();
            b.Property(p => p.CNH).IsRequired();

            b.HasOne(dp => dp.CNHType)
             .WithMany(cnh => cnh.DeliveryPersons)
             .HasForeignKey(dp => dp.CNHTypeId)
             .IsRequired();

            b.HasOne(dp => dp.User)
             .WithOne(u => u.DeliveryPerson)
             .HasForeignKey<User>(u => u.DeliveryPersonId)
             .OnDelete(DeleteBehavior.SetNull);
        }

        private void SetupDelivery(ModelBuilder modelBuilder)
        {
            SetupDefaultFields<Delivery>(modelBuilder);
            var b = modelBuilder.Entity<Delivery>();
            b.Property(p => p.PickupDate).IsRequired();

            b.HasOne(dp => dp.DeliveryPerson)
             .WithMany(cnh => cnh.Deliveries)
             .HasForeignKey(dp => dp.DeliveryPersonId)
             .IsRequired();
        }

        private void SetupVehicle(ModelBuilder modelBuilder)
        {
            SetupDefaultFields<Vehicle>(modelBuilder);
            var b = modelBuilder.Entity<Vehicle>();
            b.HasIndex(p => p.VIN).IsUnique();
            b.Property(p => p.VIN).IsRequired();
            b.Property(p => p.Model).IsRequired();
            b.Property(p => p.Year).IsRequired();
        }

        private List<UserType> SetupUserType(ModelBuilder modelBuilder)
        {
            var accountTypes = new List<UserType>
            {
                new UserType
                {
                    Id = Guid.NewGuid(),
                    Description = "ADMIN",
                    CreatedOn = DateTime.UtcNow
                },
                new UserType
                {
                    Id = Guid.NewGuid(),
                    Description = "USER",
                    CreatedOn = DateTime.UtcNow
                }
            };

            SetupDefaultFields<UserType>(modelBuilder);
            var b = modelBuilder.Entity<UserType>();
            b.HasMany(ut => ut.Users).WithOne(u => u.UserType)
                                     .HasForeignKey(u => u.UserTypeId);

            b.HasData(accountTypes);
            return accountTypes;
        }

        private void SetupUser(ModelBuilder modelBuilder, List<UserType> types)
        {
            SetupDefaultFields<User>(modelBuilder);

            var b = modelBuilder.Entity<User>();
            b.Property(s => s.Name)
             .HasMaxLength(200)
             .IsRequired();
            b.Property(s => s.Email)
             .HasMaxLength(100)
             .IsRequired();
            
            b.Property(s => s.BirthDate).IsRequired();
            b.Property(s => s.Password).HasMaxLength(200).IsRequired();
            b.Property(s => s.Enabled).HasDefaultValue(true);

            b.HasOne(u => u.UserType)
             .WithMany(ut => ut.Users)
             .HasForeignKey(u => u.UserTypeId);

            b.HasOne(u => u.DeliveryPerson)
             .WithOne(dp => dp.User)
             .HasForeignKey<DeliveryPerson>(dp => dp.UserId)
             .OnDelete(DeleteBehavior.SetNull);

            b.HasData(new User
            {
                Id = Guid.NewGuid(),
                Name = "SysAdmin",
                Email = "sysadmin@desafiobackend.com",
                Password = PasswordHelper.HashPassword("password@1"),
                Enabled = true,
                UserTypeId = types!.First(t => t.Description!.Contains("admin", StringComparison.InvariantCultureIgnoreCase)).Id,
                CreatedOn = DateTime.UtcNow
            });
        }

        #endregion
    }
}