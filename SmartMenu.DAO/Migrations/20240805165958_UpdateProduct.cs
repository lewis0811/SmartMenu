using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class UpdateProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductPriceCurrency",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductPriceCurrency",
                table: "Products");
        }
    }
}
