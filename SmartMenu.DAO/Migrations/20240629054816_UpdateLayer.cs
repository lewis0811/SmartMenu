using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class UpdateLayer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Collections_Brands_BrandId",
                table: "Collections");

            migrationBuilder.DropForeignKey(
                name: "FK_Layers_LayersItem_LayerItemId",
                table: "Layers");

            migrationBuilder.DropForeignKey(
                name: "FK_Menus_Brands_BrandId",
                table: "Menus");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreDevices_Stores_StoreId",
                table: "StoreDevices");

            migrationBuilder.DropForeignKey(
                name: "FK_Templates_Brands_BrandId",
                table: "Templates");

            migrationBuilder.DropIndex(
                name: "IX_Templates_BrandId",
                table: "Templates");

            migrationBuilder.DropIndex(
                name: "IX_StoreDevices_StoreId",
                table: "StoreDevices");

            migrationBuilder.DropIndex(
                name: "IX_Menus_BrandId",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Layers_LayerItemId",
                table: "Layers");

            migrationBuilder.DropIndex(
                name: "IX_Collections_BrandId",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "LayerItemId",
                table: "Layers");

            migrationBuilder.AddColumn<int>(
                name: "LayerId",
                table: "LayersItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LayersItem_LayerId",
                table: "LayersItem",
                column: "LayerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LayersItem_Layers_LayerId",
                table: "LayersItem",
                column: "LayerId",
                principalTable: "Layers",
                principalColumn: "LayerId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LayersItem_Layers_LayerId",
                table: "LayersItem");

            migrationBuilder.DropIndex(
                name: "IX_LayersItem_LayerId",
                table: "LayersItem");

            migrationBuilder.DropColumn(
                name: "LayerId",
                table: "LayersItem");

            migrationBuilder.AddColumn<int>(
                name: "LayerItemId",
                table: "Layers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Templates_BrandId",
                table: "Templates",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreDevices_StoreId",
                table: "StoreDevices",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_BrandId",
                table: "Menus",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Layers_LayerItemId",
                table: "Layers",
                column: "LayerItemId");

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
                name: "FK_Layers_LayersItem_LayerItemId",
                table: "Layers",
                column: "LayerItemId",
                principalTable: "LayersItem",
                principalColumn: "LayerItemId",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Templates_Brands_BrandId",
                table: "Templates",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "BrandId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
