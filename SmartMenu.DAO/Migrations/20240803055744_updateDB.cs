using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class updateDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoxItems_Fonts_BFontId",
                table: "BoxItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fonts",
                table: "Fonts");

            migrationBuilder.RenameTable(
                name: "Fonts",
                newName: "BFonts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BFonts",
                table: "BFonts",
                column: "BFontId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoxItems_BFonts_BFontId",
                table: "BoxItems",
                column: "BFontId",
                principalTable: "BFonts",
                principalColumn: "BFontId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoxItems_BFonts_BFontId",
                table: "BoxItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BFonts",
                table: "BFonts");

            migrationBuilder.RenameTable(
                name: "BFonts",
                newName: "Fonts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fonts",
                table: "Fonts",
                column: "BFontId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoxItems_Fonts_BFontId",
                table: "BoxItems",
                column: "BFontId",
                principalTable: "Fonts",
                principalColumn: "BFontId");
        }
    }
}
