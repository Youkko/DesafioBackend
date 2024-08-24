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
                    { new Guid("00d6f99c-1a45-41f1-8e13-21584520f999"), new DateTime(2024, 8, 24, 5, 51, 21, 930, DateTimeKind.Utc).AddTicks(86), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A" },
                    { new Guid("bd461824-b4b7-4654-b2fb-f52d8286b121"), new DateTime(2024, 8, 24, 5, 51, 21, 930, DateTimeKind.Utc).AddTicks(90), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "B" },
                    { new Guid("ea459095-0ef4-4645-8ecc-3c25d33387ef"), new DateTime(2024, 8, 24, 5, 51, 21, 930, DateTimeKind.Utc).AddTicks(92), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AB" }
                });

            migrationBuilder.InsertData(
                table: "RentalPlan",
                columns: new[] { "Id", "CreatedOn", "Days", "ModifiedOn", "Value" },
                values: new object[,]
                {
                    { new Guid("25300b6f-7805-4bc3-a04b-9af8c3c7f496"), new DateTime(2024, 8, 24, 5, 51, 21, 930, DateTimeKind.Utc).AddTicks(1146), 50, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 18.0 },
                    { new Guid("2bb1c1c2-bfa3-4802-bd40-4c473bb2aab7"), new DateTime(2024, 8, 24, 5, 51, 21, 930, DateTimeKind.Utc).AddTicks(1135), 7, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30.0 },
                    { new Guid("63391fb6-825e-495e-964b-b671c237e25a"), new DateTime(2024, 8, 24, 5, 51, 21, 930, DateTimeKind.Utc).AddTicks(1141), 30, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 22.0 },
                    { new Guid("7cafc884-5e19-45fa-a773-0e03f01c99cb"), new DateTime(2024, 8, 24, 5, 51, 21, 930, DateTimeKind.Utc).AddTicks(1142), 45, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 20.0 },
                    { new Guid("7dca1c50-ba51-4e3c-955a-eab2a0b11a5a"), new DateTime(2024, 8, 24, 5, 51, 21, 930, DateTimeKind.Utc).AddTicks(1139), 15, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 28.0 }
                });

            migrationBuilder.InsertData(
                table: "UserType",
                columns: new[] { "Id", "CreatedOn", "Description", "ModifiedOn" },
                values: new object[,]
                {
                    { new Guid("51c11091-178a-4db5-9758-aef0b607b136"), new DateTime(2024, 8, 24, 5, 51, 21, 931, DateTimeKind.Utc).AddTicks(2399), "USER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("c577affb-5437-4122-95cc-24d2de366ead"), new DateTime(2024, 8, 24, 5, 51, 21, 931, DateTimeKind.Utc).AddTicks(2394), "ADMIN", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "BirthDate", "CreatedOn", "DeliveryPersonId", "Email", "Enabled", "ModifiedOn", "Name", "Password", "UserTypeId" },
                values: new object[] { new Guid("4efb5e00-0c30-4c98-87c3-45bb972201b5"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 8, 24, 5, 51, 21, 938, DateTimeKind.Utc).AddTicks(7529), null, "sysadmin@desafiobackend.com", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SysAdmin", "yGg/SxouE9XP/ocO6R389wuZq6yF4C5l0KtKiSTZvUZb9Sg3sVww2YaDr55e2pgW", new Guid("c577affb-5437-4122-95cc-24d2de366ead") });

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
                name: "Rental");

            migrationBuilder.DropTable(
                name: "DeliveryPerson");

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
