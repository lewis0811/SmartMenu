using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class UpdateStoreDevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextFormat",
                table: "BoxItems");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "StoreDevices",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "StoreDevices");

            migrationBuilder.AddColumn<int>(
                name: "TextFormat",
                table: "BoxItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
