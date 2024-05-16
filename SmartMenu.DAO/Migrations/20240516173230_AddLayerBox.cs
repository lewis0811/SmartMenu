using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class AddLayerBox : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "Fonts",
                columns: table => new
                {
                    FontID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FontName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FontPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fonts", x => x.FontID);
                });

            migrationBuilder.CreateTable(
                name: "Boxes",
                columns: table => new
                {
                    BoxID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LayerID = table.Column<int>(type: "int", nullable: false),
                    FontID = table.Column<int>(type: "int", nullable: false),
                    BoxContentID = table.Column<int>(type: "int", nullable: false),
                    FontSize = table.Column<double>(type: "float", nullable: false),
                    BoxType = table.Column<int>(type: "int", nullable: false),
                    BoxColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BoxPositionX = table.Column<double>(type: "float", nullable: false),
                    BoxPositionY = table.Column<double>(type: "float", nullable: false),
                    BoxMaxCapacity = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boxes", x => x.BoxID);
                    table.ForeignKey(
                        name: "FK_Boxes_BoxContent_BoxContentID",
                        column: x => x.BoxContentID,
                        principalTable: "BoxContent",
                        principalColumn: "BoxContentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Boxes_Fonts_FontID",
                        column: x => x.FontID,
                        principalTable: "Fonts",
                        principalColumn: "FontID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Boxes_Layers_LayerID",
                        column: x => x.LayerID,
                        principalTable: "Layers",
                        principalColumn: "LayerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boxes_BoxContentID",
                table: "Boxes",
                column: "BoxContentID");

            migrationBuilder.CreateIndex(
                name: "IX_Boxes_FontID",
                table: "Boxes",
                column: "FontID");

            migrationBuilder.CreateIndex(
                name: "IX_Boxes_LayerID",
                table: "Boxes",
                column: "LayerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Boxes");

            migrationBuilder.DropTable(
                name: "BoxContent");

            migrationBuilder.DropTable(
                name: "Fonts");
        }
    }
}
