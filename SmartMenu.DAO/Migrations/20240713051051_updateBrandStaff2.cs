using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class updateBrandStaff2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "BrandStaffs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrandStaffs_StoreId",
                table: "BrandStaffs",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_BrandStaffs_Stores_StoreId",
                table: "BrandStaffs",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "StoreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrandStaffs_Stores_StoreId",
                table: "BrandStaffs");

            migrationBuilder.DropIndex(
                name: "IX_BrandStaffs_StoreId",
                table: "BrandStaffs");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "BrandStaffs");
        }
    }
}
