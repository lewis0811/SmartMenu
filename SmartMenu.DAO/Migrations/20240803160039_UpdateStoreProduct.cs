using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class UpdateStoreProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Collections_Brands_BrandId",
                table: "Collections");

            migrationBuilder.DropForeignKey(
                name: "FK_Menus_Brands_BrandId",
                table: "Menus");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreDevices_Stores_StoreId",
                table: "StoreDevices");

            migrationBuilder.DropIndex(
                name: "IX_StoreDevices_StoreId",
                table: "StoreDevices");

            migrationBuilder.DropIndex(
                name: "IX_Menus_BrandId",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Collections_BrandId",
                table: "Collections");

            migrationBuilder.AddColumn<bool>(
                name: "IconEnable",
                table: "StoreProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconEnable",
                table: "StoreProducts");

            migrationBuilder.CreateIndex(
                name: "IX_StoreDevices_StoreId",
                table: "StoreDevices",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_BrandId",
                table: "Menus",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_BrandId",
                table: "Collections",
                column: "BrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Collections_Brands_BrandId",
                table: "Collections",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "BrandId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_Brands_BrandId",
                table: "Menus",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "BrandId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreDevices_Stores_StoreId",
                table: "StoreDevices",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "StoreId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
