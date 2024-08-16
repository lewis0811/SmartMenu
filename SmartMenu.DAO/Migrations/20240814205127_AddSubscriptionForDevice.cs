using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class AddSubscriptionForDevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    SubscriptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DayDuration = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.SubscriptionId);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Payment_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Payment_Status = table.Column<int>(type: "int", nullable: false),
                    PayType = table.Column<int>(type: "int", nullable: false),
                    PayDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                });

            migrationBuilder.CreateTable(
                name: "DeviceSubscriptions",
                columns: table => new
                {
                    DeviceSubscriptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreDeviceId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubscriptionEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubscriptionStatus = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceSubscriptions", x => x.DeviceSubscriptionId);
                    table.ForeignKey(
                        name: "FK_DeviceSubscriptions_StoreDevices_StoreDeviceId",
                        column: x => x.StoreDeviceId,
                        principalTable: "StoreDevices",
                        principalColumn: "StoreDeviceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceSubscriptions_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "SubscriptionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceSubscriptions_StoreDeviceId",
                table: "DeviceSubscriptions",
                column: "StoreDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceSubscriptions_SubscriptionId",
                table: "DeviceSubscriptions",
                column: "SubscriptionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceSubscriptions");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Subscriptions");
        }
    }
}
