using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class OptimizeDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DisplayItem_Boxes_BoxID",
                table: "DisplayItem");

            migrationBuilder.DropForeignKey(
                name: "FK_DisplayItem_Displays_DisplayID",
                table: "DisplayItem");

            migrationBuilder.DropForeignKey(
                name: "FK_DisplayItem_ProductGroups_ProductGroupID",
                table: "DisplayItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Displays_Collections_CollectionID",
                table: "Displays");

            migrationBuilder.DropForeignKey(
                name: "FK_Displays_Menus_MenuID",
                table: "Displays");

            migrationBuilder.DropForeignKey(
                name: "FK_Displays_StoreDevices_StoreDeviceID",
                table: "Displays");

            migrationBuilder.DropForeignKey(
                name: "FK_Displays_Templates_TemplateID",
                table: "Displays");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreDevices_Stores_StoreID",
                table: "StoreDevices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DisplayItem",
                table: "DisplayItem");

            migrationBuilder.RenameTable(
                name: "DisplayItem",
                newName: "DisplayItems");

            migrationBuilder.RenameColumn(
                name: "StoreID",
                table: "StoreDevices",
                newName: "StoreId");

            migrationBuilder.RenameColumn(
                name: "StoreDeviceID",
                table: "StoreDevices",
                newName: "StoreDeviceId");

            migrationBuilder.RenameIndex(
                name: "IX_StoreDevices_StoreID",
                table: "StoreDevices",
                newName: "IX_StoreDevices_StoreId");

            migrationBuilder.RenameColumn(
                name: "TemplateID",
                table: "Displays",
                newName: "TemplateId");

            migrationBuilder.RenameColumn(
                name: "StoreDeviceID",
                table: "Displays",
                newName: "StoreDeviceId");

            migrationBuilder.RenameColumn(
                name: "MenuID",
                table: "Displays",
                newName: "MenuId");

            migrationBuilder.RenameColumn(
                name: "CollectionID",
                table: "Displays",
                newName: "CollectionId");

            migrationBuilder.RenameColumn(
                name: "DisplayID",
                table: "Displays",
                newName: "DisplayId");

            migrationBuilder.RenameIndex(
                name: "IX_Displays_TemplateID",
                table: "Displays",
                newName: "IX_Displays_TemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_Displays_StoreDeviceID",
                table: "Displays",
                newName: "IX_Displays_StoreDeviceId");

            migrationBuilder.RenameIndex(
                name: "IX_Displays_MenuID",
                table: "Displays",
                newName: "IX_Displays_MenuId");

            migrationBuilder.RenameIndex(
                name: "IX_Displays_CollectionID",
                table: "Displays",
                newName: "IX_Displays_CollectionId");

            migrationBuilder.RenameColumn(
                name: "BrandID",
                table: "Brands",
                newName: "BrandId");

            migrationBuilder.RenameColumn(
                name: "ProductGroupID",
                table: "DisplayItems",
                newName: "ProductGroupId");

            migrationBuilder.RenameColumn(
                name: "DisplayID",
                table: "DisplayItems",
                newName: "DisplayId");

            migrationBuilder.RenameColumn(
                name: "BoxID",
                table: "DisplayItems",
                newName: "BoxId");

            migrationBuilder.RenameColumn(
                name: "DisplayItemID",
                table: "DisplayItems",
                newName: "DisplayItemId");

            migrationBuilder.RenameIndex(
                name: "IX_DisplayItem_ProductGroupID",
                table: "DisplayItems",
                newName: "IX_DisplayItems_ProductGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_DisplayItem_DisplayID",
                table: "DisplayItems",
                newName: "IX_DisplayItems_DisplayId");

            migrationBuilder.RenameIndex(
                name: "IX_DisplayItem_BoxID",
                table: "DisplayItems",
                newName: "IX_DisplayItems_BoxId");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayImgPath",
                table: "Displays",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ProductGroupId",
                table: "DisplayItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DisplayItems",
                table: "DisplayItems",
                column: "DisplayItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_DisplayItems_Boxes_BoxId",
                table: "DisplayItems",
                column: "BoxId",
                principalTable: "Boxes",
                principalColumn: "BoxID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DisplayItems_Displays_DisplayId",
                table: "DisplayItems",
                column: "DisplayId",
                principalTable: "Displays",
                principalColumn: "DisplayId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DisplayItems_ProductGroups_ProductGroupId",
                table: "DisplayItems",
                column: "ProductGroupId",
                principalTable: "ProductGroups",
                principalColumn: "ProductGroupID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Displays_Collections_CollectionId",
                table: "Displays",
                column: "CollectionId",
                principalTable: "Collections",
                principalColumn: "CollectionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Displays_Menus_MenuId",
                table: "Displays",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "MenuID");

            migrationBuilder.AddForeignKey(
                name: "FK_Displays_StoreDevices_StoreDeviceId",
                table: "Displays",
                column: "StoreDeviceId",
                principalTable: "StoreDevices",
                principalColumn: "StoreDeviceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Displays_Templates_TemplateId",
                table: "Displays",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "TemplateID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreDevices_Stores_StoreId",
                table: "StoreDevices",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "StoreID",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DisplayItems_Boxes_BoxId",
                table: "DisplayItems");

            migrationBuilder.DropForeignKey(
                name: "FK_DisplayItems_Displays_DisplayId",
                table: "DisplayItems");

            migrationBuilder.DropForeignKey(
                name: "FK_DisplayItems_ProductGroups_ProductGroupId",
                table: "DisplayItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Displays_Collections_CollectionId",
                table: "Displays");

            migrationBuilder.DropForeignKey(
                name: "FK_Displays_Menus_MenuId",
                table: "Displays");

            migrationBuilder.DropForeignKey(
                name: "FK_Displays_StoreDevices_StoreDeviceId",
                table: "Displays");

            migrationBuilder.DropForeignKey(
                name: "FK_Displays_Templates_TemplateId",
                table: "Displays");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreDevices_Stores_StoreId",
                table: "StoreDevices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DisplayItems",
                table: "DisplayItems");

            migrationBuilder.RenameTable(
                name: "DisplayItems",
                newName: "DisplayItem");

            migrationBuilder.RenameColumn(
                name: "StoreId",
                table: "StoreDevices",
                newName: "StoreID");

            migrationBuilder.RenameColumn(
                name: "StoreDeviceId",
                table: "StoreDevices",
                newName: "StoreDeviceID");

            migrationBuilder.RenameIndex(
                name: "IX_StoreDevices_StoreId",
                table: "StoreDevices",
                newName: "IX_StoreDevices_StoreID");

            migrationBuilder.RenameColumn(
                name: "TemplateId",
                table: "Displays",
                newName: "TemplateID");

            migrationBuilder.RenameColumn(
                name: "StoreDeviceId",
                table: "Displays",
                newName: "StoreDeviceID");

            migrationBuilder.RenameColumn(
                name: "MenuId",
                table: "Displays",
                newName: "MenuID");

            migrationBuilder.RenameColumn(
                name: "CollectionId",
                table: "Displays",
                newName: "CollectionID");

            migrationBuilder.RenameColumn(
                name: "DisplayId",
                table: "Displays",
                newName: "DisplayID");

            migrationBuilder.RenameIndex(
                name: "IX_Displays_TemplateId",
                table: "Displays",
                newName: "IX_Displays_TemplateID");

            migrationBuilder.RenameIndex(
                name: "IX_Displays_StoreDeviceId",
                table: "Displays",
                newName: "IX_Displays_StoreDeviceID");

            migrationBuilder.RenameIndex(
                name: "IX_Displays_MenuId",
                table: "Displays",
                newName: "IX_Displays_MenuID");

            migrationBuilder.RenameIndex(
                name: "IX_Displays_CollectionId",
                table: "Displays",
                newName: "IX_Displays_CollectionID");

            migrationBuilder.RenameColumn(
                name: "BrandId",
                table: "Brands",
                newName: "BrandID");

            migrationBuilder.RenameColumn(
                name: "ProductGroupId",
                table: "DisplayItem",
                newName: "ProductGroupID");

            migrationBuilder.RenameColumn(
                name: "DisplayId",
                table: "DisplayItem",
                newName: "DisplayID");

            migrationBuilder.RenameColumn(
                name: "BoxId",
                table: "DisplayItem",
                newName: "BoxID");

            migrationBuilder.RenameColumn(
                name: "DisplayItemId",
                table: "DisplayItem",
                newName: "DisplayItemID");

            migrationBuilder.RenameIndex(
                name: "IX_DisplayItems_ProductGroupId",
                table: "DisplayItem",
                newName: "IX_DisplayItem_ProductGroupID");

            migrationBuilder.RenameIndex(
                name: "IX_DisplayItems_DisplayId",
                table: "DisplayItem",
                newName: "IX_DisplayItem_DisplayID");

            migrationBuilder.RenameIndex(
                name: "IX_DisplayItems_BoxId",
                table: "DisplayItem",
                newName: "IX_DisplayItem_BoxID");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayImgPath",
                table: "Displays",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductGroupID",
                table: "DisplayItem",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DisplayItem",
                table: "DisplayItem",
                column: "DisplayItemID");

            migrationBuilder.AddForeignKey(
                name: "FK_DisplayItem_Boxes_BoxID",
                table: "DisplayItem",
                column: "BoxID",
                principalTable: "Boxes",
                principalColumn: "BoxID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DisplayItem_Displays_DisplayID",
                table: "DisplayItem",
                column: "DisplayID",
                principalTable: "Displays",
                principalColumn: "DisplayID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DisplayItem_ProductGroups_ProductGroupID",
                table: "DisplayItem",
                column: "ProductGroupID",
                principalTable: "ProductGroups",
                principalColumn: "ProductGroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_Displays_Collections_CollectionID",
                table: "Displays",
                column: "CollectionID",
                principalTable: "Collections",
                principalColumn: "CollectionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Displays_Menus_MenuID",
                table: "Displays",
                column: "MenuID",
                principalTable: "Menus",
                principalColumn: "MenuID");

            migrationBuilder.AddForeignKey(
                name: "FK_Displays_StoreDevices_StoreDeviceID",
                table: "Displays",
                column: "StoreDeviceID",
                principalTable: "StoreDevices",
                principalColumn: "StoreDeviceID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Displays_Templates_TemplateID",
                table: "Displays",
                column: "TemplateID",
                principalTable: "Templates",
                principalColumn: "TemplateID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreDevices_Stores_StoreID",
                table: "StoreDevices",
                column: "StoreID",
                principalTable: "Stores",
                principalColumn: "StoreID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
