using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotorcycleRental.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CNHType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Type = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CNHType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Motorcycle",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    VIN = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Brand = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motorcycle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Message = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CustomerName = table.Column<string>(type: "text", nullable: true),
                    CustomerEmail = table.Column<string>(type: "text", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RentalPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Days = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalPlan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rental",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    MotorcycleId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RentalStatus = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rental", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rental_Motorcycle_MotorcycleId",
                        column: x => x.MotorcycleId,
                        principalTable: "Motorcycle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItem_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Password = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    UserTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_UserType_UserTypeId",
                        column: x => x.UserTypeId,
                        principalTable: "UserType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryPerson",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CNPJ = table.Column<string>(type: "text", nullable: false),
                    CNH = table.Column<string>(type: "text", nullable: false),
                    CNHTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryPerson", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryPerson_CNHType_CNHTypeId",
                        column: x => x.CNHTypeId,
                        principalTable: "CNHType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryPerson_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Delivery",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    DeliveryPersonId = table.Column<Guid>(type: "uuid", nullable: false),
                    PickupDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DropoffDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Delivery", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Delivery_DeliveryPerson_DeliveryPersonId",
                        column: x => x.DeliveryPersonId,
                        principalTable: "DeliveryPerson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CNHType",
                columns: new[] { "Id", "CreatedOn", "ModifiedOn", "Type" },
                values: new object[,]
                {
                    { new Guid("505220b8-7133-4df7-9b9b-7df22e8ff6ea"), new DateTime(2024, 8, 23, 6, 15, 56, 197, DateTimeKind.Utc).AddTicks(6036), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A" },
                    { new Guid("ba980615-c218-4caa-8b10-224e4eb2545a"), new DateTime(2024, 8, 23, 6, 15, 56, 197, DateTimeKind.Utc).AddTicks(6053), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AB" },
                    { new Guid("c0946d17-e372-4278-abca-0bf8698e8cb8"), new DateTime(2024, 8, 23, 6, 15, 56, 197, DateTimeKind.Utc).AddTicks(6041), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "B" }
                });

            migrationBuilder.InsertData(
                table: "RentalPlan",
                columns: new[] { "Id", "CreatedOn", "Days", "ModifiedOn", "Value" },
                values: new object[,]
                {
                    { new Guid("2d9cd8a7-aba5-411b-ab9f-1700266b0da9"), new DateTime(2024, 8, 23, 6, 15, 56, 197, DateTimeKind.Utc).AddTicks(7064), 50, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 18.0 },
                    { new Guid("3c558cd8-2571-4d95-ab00-9ba2f7a6421a"), new DateTime(2024, 8, 23, 6, 15, 56, 197, DateTimeKind.Utc).AddTicks(7056), 15, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 28.0 },
                    { new Guid("b713dcad-6351-4c1a-a622-d32ef20d3c0a"), new DateTime(2024, 8, 23, 6, 15, 56, 197, DateTimeKind.Utc).AddTicks(7057), 30, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 22.0 },
                    { new Guid("c849a633-ce44-4c67-a1d6-bb74847d1538"), new DateTime(2024, 8, 23, 6, 15, 56, 197, DateTimeKind.Utc).AddTicks(7052), 7, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30.0 },
                    { new Guid("f31993a0-03c7-4df5-bde4-3a643514c614"), new DateTime(2024, 8, 23, 6, 15, 56, 197, DateTimeKind.Utc).AddTicks(7063), 45, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 20.0 }
                });

            migrationBuilder.InsertData(
                table: "UserType",
                columns: new[] { "Id", "CreatedOn", "Description", "ModifiedOn" },
                values: new object[,]
                {
                    { new Guid("1f2b9e27-5fea-481a-84d9-ec2ac83d13d7"), new DateTime(2024, 8, 23, 6, 15, 56, 198, DateTimeKind.Utc).AddTicks(581), "USER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("29dcbbe6-d2eb-4c56-9ad7-331ed7a2a55f"), new DateTime(2024, 8, 23, 6, 15, 56, 198, DateTimeKind.Utc).AddTicks(574), "ADMIN", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "BirthDate", "CreatedOn", "Email", "Enabled", "ModifiedOn", "Name", "Password", "UserTypeId" },
                values: new object[] { new Guid("f1736674-a197-4de4-ad15-7adac6a29627"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 8, 23, 6, 15, 56, 205, DateTimeKind.Utc).AddTicks(1079), "sysadmin@desafiobackend.com", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SysAdmin", "61duyL+UTBCL47MIh66Fr3ib8tYo674LVYAL+c1hCdHINuI2WVWghEI9x4MT7OGK", new Guid("29dcbbe6-d2eb-4c56-9ad7-331ed7a2a55f") });

            migrationBuilder.CreateIndex(
                name: "IX_CNHType_Type",
                table: "CNHType",
                column: "Type",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_DeliveryPersonId",
                table: "Delivery",
                column: "DeliveryPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPerson_CNH",
                table: "DeliveryPerson",
                column: "CNH",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPerson_CNHTypeId",
                table: "DeliveryPerson",
                column: "CNHTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPerson_CNPJ",
                table: "DeliveryPerson",
                column: "CNPJ",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPerson_UserId",
                table: "DeliveryPerson",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Motorcycle_VIN",
                table: "Motorcycle",
                column: "VIN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId",
                table: "OrderItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Rental_MotorcycleId",
                table: "Rental",
                column: "MotorcycleId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalPlan_Days",
                table: "RentalPlan",
                column: "Days",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_UserTypeId",
                table: "User",
                column: "UserTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Delivery");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "Rental");

            migrationBuilder.DropTable(
                name: "RentalPlan");

            migrationBuilder.DropTable(
                name: "DeliveryPerson");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Motorcycle");

            migrationBuilder.DropTable(
                name: "CNHType");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "UserType");
        }
    }
}
