using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class UpdateDeviceSubscription2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceSubscriptions_Subscriptions_SubscriptionId",
                table: "DeviceSubscriptions");

            migrationBuilder.AlterColumn<int>(
                name: "SubscriptionId",
                table: "DeviceSubscriptions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceSubscriptions_Subscriptions_SubscriptionId",
                table: "DeviceSubscriptions",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "SubscriptionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceSubscriptions_Subscriptions_SubscriptionId",
                table: "DeviceSubscriptions");

            migrationBuilder.AlterColumn<int>(
                name: "SubscriptionId",
                table: "DeviceSubscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
