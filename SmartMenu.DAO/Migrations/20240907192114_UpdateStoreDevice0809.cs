using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class UpdateStoreDevice0809 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceLocation",
                table: "StoreDevices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StoreDevices_StoreId",
                table: "StoreDevices",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreDevices_Stores_StoreId",
                table: "StoreDevices",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "StoreId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreDevices_Stores_StoreId",
                table: "StoreDevices");

            migrationBuilder.DropIndex(
                name: "IX_StoreDevices_StoreId",
                table: "StoreDevices");

            migrationBuilder.DropColumn(
                name: "DeviceLocation",
                table: "StoreDevices");
        }
    }
}
