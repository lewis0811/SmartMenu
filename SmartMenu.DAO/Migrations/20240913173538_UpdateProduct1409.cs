using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class UpdateProduct1409 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatioType",
                table: "StoreDevices");

            migrationBuilder.DropColumn(
                name: "ProductLogoPath",
                table: "Products");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RatioType",
                table: "StoreDevices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProductLogoPath",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
