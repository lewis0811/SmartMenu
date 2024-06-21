using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class addImgPath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoxItems_Fonts_FontID",
                table: "BoxItems");

            migrationBuilder.RenameColumn(
                name: "FontID",
                table: "BoxItems",
                newName: "FontId");

            migrationBuilder.RenameIndex(
                name: "IX_BoxItems_FontID",
                table: "BoxItems",
                newName: "IX_BoxItems_FontId");

            migrationBuilder.AddColumn<string>(
                name: "TemplateImgPath",
                table: "Templates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisplayImgPath",
                table: "Displays",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_BoxItems_Fonts_FontId",
                table: "BoxItems",
                column: "FontId",
                principalTable: "Fonts",
                principalColumn: "FontID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoxItems_Fonts_FontId",
                table: "BoxItems");

            migrationBuilder.DropColumn(
                name: "TemplateImgPath",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "DisplayImgPath",
                table: "Displays");

            migrationBuilder.RenameColumn(
                name: "FontId",
                table: "BoxItems",
                newName: "FontID");

            migrationBuilder.RenameIndex(
                name: "IX_BoxItems_FontId",
                table: "BoxItems",
                newName: "IX_BoxItems_FontID");

            migrationBuilder.AddForeignKey(
                name: "FK_BoxItems_Fonts_FontID",
                table: "BoxItems",
                column: "FontID",
                principalTable: "Fonts",
                principalColumn: "FontID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
