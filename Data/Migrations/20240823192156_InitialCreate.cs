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
                name: "Vehicle",
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
                    table.PrimaryKey("PK_Vehicle", x => x.Id);
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
                    DeliveryPersonId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
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
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Rental",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    RentalPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rental", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rental_RentalPlan_RentalPlanId",
                        column: x => x.RentalPlanId,
                        principalTable: "RentalPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rental_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rental_Vehicle_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicle",
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
                    DropoffDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    { new Guid("33db8acf-b969-476f-bd6a-8e32359a8fea"), new DateTime(2024, 8, 23, 19, 21, 56, 457, DateTimeKind.Utc).AddTicks(4905), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AB" },
                    { new Guid("3a421083-7e4c-45e0-91ed-c7c8e6410e9d"), new DateTime(2024, 8, 23, 19, 21, 56, 457, DateTimeKind.Utc).AddTicks(4903), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "B" },
                    { new Guid("8a9c9e8e-1af1-45d3-a50e-4b8d9a6ad595"), new DateTime(2024, 8, 23, 19, 21, 56, 457, DateTimeKind.Utc).AddTicks(4898), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A" }
                });

            migrationBuilder.InsertData(
                table: "RentalPlan",
                columns: new[] { "Id", "CreatedOn", "Days", "ModifiedOn", "Value" },
                values: new object[,]
                {
                    { new Guid("2dbbeddb-2f70-46b3-a613-fdfb04dfc37a"), new DateTime(2024, 8, 23, 19, 21, 56, 457, DateTimeKind.Utc).AddTicks(5867), 15, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 28.0 },
                    { new Guid("459ab778-2742-4b02-9938-f6f86a0b0f59"), new DateTime(2024, 8, 23, 19, 21, 56, 457, DateTimeKind.Utc).AddTicks(5864), 7, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30.0 },
                    { new Guid("a622dc6c-8a66-481f-8ab5-0cedd0d3ea81"), new DateTime(2024, 8, 23, 19, 21, 56, 457, DateTimeKind.Utc).AddTicks(5871), 45, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 20.0 },
                    { new Guid("b03c2f53-425e-4bd9-8327-b9fe311cfc96"), new DateTime(2024, 8, 23, 19, 21, 56, 457, DateTimeKind.Utc).AddTicks(5869), 30, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 22.0 },
                    { new Guid("e4023d8f-bba6-4023-a4ef-f919c151f205"), new DateTime(2024, 8, 23, 19, 21, 56, 457, DateTimeKind.Utc).AddTicks(5875), 50, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 18.0 }
                });

            migrationBuilder.InsertData(
                table: "UserType",
                columns: new[] { "Id", "CreatedOn", "Description", "ModifiedOn" },
                values: new object[,]
                {
                    { new Guid("b0b16583-409d-4c03-a80d-18a66583cd2a"), new DateTime(2024, 8, 23, 19, 21, 56, 458, DateTimeKind.Utc).AddTicks(7338), "ADMIN", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("d6dbed9c-786e-48e2-b95f-35fb155c2cda"), new DateTime(2024, 8, 23, 19, 21, 56, 458, DateTimeKind.Utc).AddTicks(7342), "USER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "BirthDate", "CreatedOn", "DeliveryPersonId", "Email", "Enabled", "ModifiedOn", "Name", "Password", "UserTypeId" },
                values: new object[] { new Guid("943d2ef4-5139-43b2-80d4-8c10abe77c6c"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 8, 23, 19, 21, 56, 466, DateTimeKind.Utc).AddTicks(2353), null, "sysadmin@desafiobackend.com", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SysAdmin", "0EBtD46nJeSMPGBri/6lW4PLr7W+Uw0PhEAt2ENI2VYBC8VBbzC27C1KfyUgfRoI", new Guid("b0b16583-409d-4c03-a80d-18a66583cd2a") });

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
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId",
                table: "OrderItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Rental_RentalPlanId",
                table: "Rental",
                column: "RentalPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Rental_UserId",
                table: "Rental",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rental_VehicleId",
                table: "Rental",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalPlan_Days",
                table: "RentalPlan",
                column: "Days",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_UserTypeId",
                table: "User",
                column: "UserTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_VIN",
                table: "Vehicle",
                column: "VIN",
                unique: true);
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
                name: "DeliveryPerson");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "RentalPlan");

            migrationBuilder.DropTable(
                name: "Vehicle");

            migrationBuilder.DropTable(
                name: "CNHType");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "UserType");
        }
    }
}
