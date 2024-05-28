using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class UpdateBoxProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boxes_BoxContent_BoxContentID",
                table: "Boxes");

            

            migrationBuilder.DropTable(
                name: "BoxContent");

            migrationBuilder.DropIndex(
                name: "IX_Boxes_BoxContentID",
                table: "Boxes");

            migrationBuilder.DropColumn(
                name: "BoxContentID",
                table: "Boxes");

            migrationBuilder.AddColumn<string>(
                name: "BoxContent",
                table: "Boxes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoxContent",
                table: "Boxes");

            migrationBuilder.AddColumn<int>(
                name: "BoxContentID",
                table: "Boxes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BoxContent",
                columns: table => new
                {
                    BoxContentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxContent", x => x.BoxContentID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boxes_BoxContentID",
                table: "Boxes",
                column: "BoxContentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Boxes_BoxContent_BoxContentID",
                table: "Boxes",
                column: "BoxContentID",
                principalTable: "BoxContent",
                principalColumn: "BoxContentID",
                onDelete: ReferentialAction.Cascade);


        }
    }
}
