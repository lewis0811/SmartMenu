using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class UpdateSubscription2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceSubscriptions_Subscriptions_SubscriptionId",
                table: "DeviceSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_DeviceSubscriptions_SubscriptionId",
                table: "DeviceSubscriptions");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "DeviceSubscriptions");

            migrationBuilder.AddColumn<int>(
                name: "DeviceSubscriptionId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_DeviceSubscriptionId",
                table: "Transactions",
                column: "DeviceSubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_DeviceSubscriptions_DeviceSubscriptionId",
                table: "Transactions",
                column: "DeviceSubscriptionId",
                principalTable: "DeviceSubscriptions",
                principalColumn: "DeviceSubscriptionId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_DeviceSubscriptions_DeviceSubscriptionId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_DeviceSubscriptionId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DeviceSubscriptionId",
                table: "Transactions");

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionId",
                table: "DeviceSubscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceSubscriptions_SubscriptionId",
                table: "DeviceSubscriptions",
                column: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceSubscriptions_Subscriptions_SubscriptionId",
                table: "DeviceSubscriptions",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "SubscriptionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
