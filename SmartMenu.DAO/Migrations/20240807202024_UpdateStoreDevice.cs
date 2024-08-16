using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class UpdateStoreDevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceCode",
                table: "StoreDevices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceCode",
                table: "StoreDevices");
        }
    }
}
