using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class addProductSizePrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DisplayItem_ProductGroups_ProductGroupID",
                table: "DisplayItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductGroupsItem_ProductGroups_ProductGroupID",
                table: "ProductGroupsItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductGroupsItem_Products_ProductID",
                table: "ProductGroupsItem");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreProducts_Stores_StoreId",
                table: "StoreProducts");

            migrationBuilder.DropIndex(
                name: "IX_StoreProducts_StoreId",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "DisplayType",
                table: "StoreDevices");

            migrationBuilder.DropColumn(
                name: "ProductPrice",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "IsEnabled",
                table: "StoreProducts",
                newName: "IsAvailable");

            migrationBuilder.RenameColumn(
                name: "ProductGroupID",
                table: "ProductGroupsItem",
                newName: "ProductGroupId");

            migrationBuilder.RenameColumn(
                name: "ProductGroupItemID",
                table: "ProductGroupsItem",
                newName: "ProductGroupItemId");

            migrationBuilder.RenameColumn(
                name: "ProductID",
                table: "ProductGroupsItem",
                newName: "ProductSizePriceId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductGroupsItem_ProductGroupID",
                table: "ProductGroupsItem",
                newName: "IX_ProductGroupsItem_ProductGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductGroupsItem_ProductID",
                table: "ProductGroupsItem",
                newName: "IX_ProductGroupsItem_ProductSizePriceId");

            migrationBuilder.AddColumn<float>(
                name: "DeviceHeight",
                table: "StoreDevices",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "DeviceWidth",
                table: "StoreDevices",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<double>(
                name: "EndingHour",
                table: "Displays",
                type: "float",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductGroupID",
                table: "DisplayItem",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "BoxItems",
                columns: table => new
                {
                    BoxItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoxId = table.Column<int>(type: "int", nullable: false),
                    FontID = table.Column<int>(type: "int", nullable: false),
                    FontSize = table.Column<double>(type: "float", nullable: false),
                    TextFormat = table.Column<int>(type: "int", nullable: false),
                    BoxType = table.Column<int>(type: "int", nullable: false),
                    BoxColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxItems", x => x.BoxItemId);
                    table.ForeignKey(
                        name: "FK_BoxItems_Boxes_BoxId",
                        column: x => x.BoxId,
                        principalTable: "Boxes",
                        principalColumn: "BoxID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoxItems_Fonts_FontID",
                        column: x => x.FontID,
                        principalTable: "Fonts",
                        principalColumn: "FontID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductSizes",
                columns: table => new
                {
                    ProductSizeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SizeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSizes", x => x.ProductSizeId);
                });

            migrationBuilder.CreateTable(
                name: "ProductSizePrices",
                columns: table => new
                {
                    ProductSizePriceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductSizeId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSizePrices", x => x.ProductSizePriceId);
                    table.ForeignKey(
                        name: "FK_ProductSizePrices_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductSizePrices_ProductSizes_ProductSizeId",
                        column: x => x.ProductSizeId,
                        principalTable: "ProductSizes",
                        principalColumn: "ProductSizeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoxItems_BoxId",
                table: "BoxItems",
                column: "BoxId");

            migrationBuilder.CreateIndex(
                name: "IX_BoxItems_FontID",
                table: "BoxItems",
                column: "FontID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizePrices_ProductId",
                table: "ProductSizePrices",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizePrices_ProductSizeId",
                table: "ProductSizePrices",
                column: "ProductSizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DisplayItem_ProductGroups_ProductGroupID",
                table: "DisplayItem",
                column: "ProductGroupID",
                principalTable: "ProductGroups",
                principalColumn: "ProductGroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductGroupsItem_ProductGroups_ProductGroupId",
                table: "ProductGroupsItem",
                column: "ProductGroupId",
                principalTable: "ProductGroups",
                principalColumn: "ProductGroupID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductGroupsItem_ProductSizePrices_ProductSizePriceId",
                table: "ProductGroupsItem",
                column: "ProductSizePriceId",
                principalTable: "ProductSizePrices",
                principalColumn: "ProductSizePriceId",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DisplayItem_ProductGroups_ProductGroupID",
                table: "DisplayItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductGroupsItem_ProductGroups_ProductGroupId",
                table: "ProductGroupsItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductGroupsItem_ProductSizePrices_ProductSizePriceId",
                table: "ProductGroupsItem");

            migrationBuilder.DropTable(
                name: "BoxItems");

            migrationBuilder.DropTable(
                name: "ProductSizePrices");

            migrationBuilder.DropTable(
                name: "ProductSizes");

            migrationBuilder.DropColumn(
                name: "DeviceHeight",
                table: "StoreDevices");

            migrationBuilder.DropColumn(
                name: "DeviceWidth",
                table: "StoreDevices");

            migrationBuilder.DropColumn(
                name: "EndingHour",
                table: "Displays");

            migrationBuilder.RenameColumn(
                name: "IsAvailable",
                table: "StoreProducts",
                newName: "IsEnabled");

            migrationBuilder.RenameColumn(
                name: "ProductGroupId",
                table: "ProductGroupsItem",
                newName: "ProductGroupID");

            migrationBuilder.RenameColumn(
                name: "ProductGroupItemId",
                table: "ProductGroupsItem",
                newName: "ProductGroupItemID");

            migrationBuilder.RenameColumn(
                name: "ProductSizePriceId",
                table: "ProductGroupsItem",
                newName: "ProductID");

            migrationBuilder.RenameIndex(
                name: "IX_ProductGroupsItem_ProductGroupId",
                table: "ProductGroupsItem",
                newName: "IX_ProductGroupsItem_ProductGroupID");

            migrationBuilder.RenameIndex(
                name: "IX_ProductGroupsItem_ProductSizePriceId",
                table: "ProductGroupsItem",
                newName: "IX_ProductGroupsItem_ProductID");

            migrationBuilder.AddColumn<int>(
                name: "DisplayType",
                table: "StoreDevices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "ProductPrice",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<int>(
                name: "ProductGroupID",
                table: "DisplayItem",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreProducts_StoreId",
                table: "StoreProducts",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_DisplayItem_ProductGroups_ProductGroupID",
                table: "DisplayItem",
                column: "ProductGroupID",
                principalTable: "ProductGroups",
                principalColumn: "ProductGroupID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductGroupsItem_ProductGroups_ProductGroupID",
                table: "ProductGroupsItem",
                column: "ProductGroupID",
                principalTable: "ProductGroups",
                principalColumn: "ProductGroupID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductGroupsItem_Products_ProductID",
                table: "ProductGroupsItem",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreProducts_Stores_StoreId",
                table: "StoreProducts",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "StoreID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
