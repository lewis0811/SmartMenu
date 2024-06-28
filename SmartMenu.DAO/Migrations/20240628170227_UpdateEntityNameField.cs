using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class UpdateEntityNameField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrandStaffs_Brands_BrandID",
                table: "BrandStaffs");

            migrationBuilder.DropForeignKey(
                name: "FK_BrandStaffs_Users_UserID",
                table: "BrandStaffs");

            migrationBuilder.RenameColumn(
                name: "LayerItemID",
                table: "LayersItem",
                newName: "LayerItemId");

            migrationBuilder.RenameColumn(
                name: "CategoryID",
                table: "Categories",
                newName: "CategoryId");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "BrandStaffs",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "BrandID",
                table: "BrandStaffs",
                newName: "BrandId");

            migrationBuilder.RenameColumn(
                name: "BrandStaffID",
                table: "BrandStaffs",
                newName: "BrandStaffId");

            migrationBuilder.RenameIndex(
                name: "IX_BrandStaffs_UserID",
                table: "BrandStaffs",
                newName: "IX_BrandStaffs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BrandStaffs_BrandID",
                table: "BrandStaffs",
                newName: "IX_BrandStaffs_BrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_BrandStaffs_Brands_BrandId",
                table: "BrandStaffs",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "BrandId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BrandStaffs_Users_UserId",
                table: "BrandStaffs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrandStaffs_Brands_BrandId",
                table: "BrandStaffs");

            migrationBuilder.DropForeignKey(
                name: "FK_BrandStaffs_Users_UserId",
                table: "BrandStaffs");

            migrationBuilder.RenameColumn(
                name: "LayerItemId",
                table: "LayersItem",
                newName: "LayerItemID");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Categories",
                newName: "CategoryID");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BrandStaffs",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "BrandId",
                table: "BrandStaffs",
                newName: "BrandID");

            migrationBuilder.RenameColumn(
                name: "BrandStaffId",
                table: "BrandStaffs",
                newName: "BrandStaffID");

            migrationBuilder.RenameIndex(
                name: "IX_BrandStaffs_UserId",
                table: "BrandStaffs",
                newName: "IX_BrandStaffs_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_BrandStaffs_BrandId",
                table: "BrandStaffs",
                newName: "IX_BrandStaffs_BrandID");

            migrationBuilder.AddForeignKey(
                name: "FK_BrandStaffs_Brands_BrandID",
                table: "BrandStaffs",
                column: "BrandID",
                principalTable: "Brands",
                principalColumn: "BrandId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BrandStaffs_Users_UserID",
                table: "BrandStaffs",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
