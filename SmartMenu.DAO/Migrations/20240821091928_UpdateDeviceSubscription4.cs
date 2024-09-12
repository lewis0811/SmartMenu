using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class UpdateDeviceSubscription4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
