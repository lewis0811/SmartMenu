using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class AddDeviceDisplayItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    ScheduleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DurationByHour = table.Column<double>(type: "float", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.ScheduleID);
                });

            migrationBuilder.CreateTable(
                name: "Displays",
                columns: table => new
                {
                    DisplayID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuID = table.Column<int>(type: "int", nullable: false),
                    CollectionID = table.Column<int>(type: "int", nullable: false),
                    TemplateID = table.Column<int>(type: "int", nullable: false),
                    ScheduleID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Displays", x => x.DisplayID);
                    table.ForeignKey(
                        name: "FK_Displays_Collections_CollectionID",
                        column: x => x.CollectionID,
                        principalTable: "Collections",
                        principalColumn: "CollectionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Displays_Menus_MenuID",
                        column: x => x.MenuID,
                        principalTable: "Menus",
                        principalColumn: "MenuID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Displays_Schedules_ScheduleID",
                        column: x => x.ScheduleID,
                        principalTable: "Schedules",
                        principalColumn: "ScheduleID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Displays_Templates_TemplateID",
                        column: x => x.TemplateID,
                        principalTable: "Templates",
                        principalColumn: "TemplateID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DisplayItem",
                columns: table => new
                {
                    DisplayItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayID = table.Column<int>(type: "int", nullable: false),
                    BoxID = table.Column<int>(type: "int", nullable: false),
                    ProductGroupID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisplayItem", x => x.DisplayItemID);
                    table.ForeignKey(
                        name: "FK_DisplayItem_Boxes_BoxID",
                        column: x => x.BoxID,
                        principalTable: "Boxes",
                        principalColumn: "BoxID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DisplayItem_Displays_DisplayID",
                        column: x => x.DisplayID,
                        principalTable: "Displays",
                        principalColumn: "DisplayID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_DisplayItem_ProductGroups_ProductGroupID",
                        column: x => x.ProductGroupID,
                        principalTable: "ProductGroups",
                        principalColumn: "ProductGroupID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreDevices",
                columns: table => new
                {
                    StoreDeviceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreID = table.Column<int>(type: "int", nullable: false),
                    DisplayID = table.Column<int>(type: "int", nullable: false),
                    StoreDeviceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayType = table.Column<int>(type: "int", nullable: false),
                    IsDisplay = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreDevices", x => x.StoreDeviceID);
                    table.ForeignKey(
                        name: "FK_StoreDevices_Displays_DisplayID",
                        column: x => x.DisplayID,
                        principalTable: "Displays",
                        principalColumn: "DisplayID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreDevices_Stores_StoreID",
                        column: x => x.StoreID,
                        principalTable: "Stores",
                        principalColumn: "StoreID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DisplayItem_BoxID",
                table: "DisplayItem",
                column: "BoxID");

            migrationBuilder.CreateIndex(
                name: "IX_DisplayItem_DisplayID",
                table: "DisplayItem",
                column: "DisplayID");

            migrationBuilder.CreateIndex(
                name: "IX_DisplayItem_ProductGroupID",
                table: "DisplayItem",
                column: "ProductGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_Displays_CollectionID",
                table: "Displays",
                column: "CollectionID");

            migrationBuilder.CreateIndex(
                name: "IX_Displays_MenuID",
                table: "Displays",
                column: "MenuID");

            migrationBuilder.CreateIndex(
                name: "IX_Displays_ScheduleID",
                table: "Displays",
                column: "ScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_Displays_TemplateID",
                table: "Displays",
                column: "TemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_StoreDevices_DisplayID",
                table: "StoreDevices",
                column: "DisplayID");

            migrationBuilder.CreateIndex(
                name: "IX_StoreDevices_StoreID",
                table: "StoreDevices",
                column: "StoreID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisplayItem");

            migrationBuilder.DropTable(
                name: "StoreDevices");

            migrationBuilder.DropTable(
                name: "Displays");

            migrationBuilder.DropTable(
                name: "Schedules");
        }
    }
}
