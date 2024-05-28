using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class updateDeviceDisplayAndTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Displays_Schedules_ScheduleID",
                table: "Displays");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreDevices_Displays_DisplayID",
                table: "StoreDevices");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_StoreDevices_DisplayID",
                table: "StoreDevices");

            migrationBuilder.DropIndex(
                name: "IX_Displays_ScheduleID",
                table: "Displays");

            migrationBuilder.DropColumn(
                name: "DisplayID",
                table: "StoreDevices");

            migrationBuilder.DropColumn(
                name: "IsDisplay",
                table: "StoreDevices");

            migrationBuilder.DropColumn(
                name: "ScheduleID",
                table: "Displays");

            migrationBuilder.AddColumn<int>(
                name: "BrandID",
                table: "Templates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "StartingHour",
                table: "Displays",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "StoreDeviceID",
                table: "Displays",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Templates_BrandID",
                table: "Templates",
                column: "BrandID");

            migrationBuilder.CreateIndex(
                name: "IX_Displays_StoreDeviceID",
                table: "Displays",
                column: "StoreDeviceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Displays_StoreDevices_StoreDeviceID",
                table: "Displays",
                column: "StoreDeviceID",
                principalTable: "StoreDevices",
                principalColumn: "StoreDeviceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Templates_Brands_BrandID",
                table: "Templates",
                column: "BrandID",
                principalTable: "Brands",
                principalColumn: "BrandID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Displays_StoreDevices_StoreDeviceID",
                table: "Displays");

            migrationBuilder.DropForeignKey(
                name: "FK_Templates_Brands_BrandID",
                table: "Templates");

            migrationBuilder.DropIndex(
                name: "IX_Templates_BrandID",
                table: "Templates");

            migrationBuilder.DropIndex(
                name: "IX_Displays_StoreDeviceID",
                table: "Displays");

            migrationBuilder.DropColumn(
                name: "BrandID",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "StartingHour",
                table: "Displays");

            migrationBuilder.DropColumn(
                name: "StoreDeviceID",
                table: "Displays");

            migrationBuilder.AddColumn<int>(
                name: "DisplayID",
                table: "StoreDevices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDisplay",
                table: "StoreDevices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ScheduleID",
                table: "Displays",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_StoreDevices_DisplayID",
                table: "StoreDevices",
                column: "DisplayID");

            migrationBuilder.CreateIndex(
                name: "IX_Displays_ScheduleID",
                table: "Displays",
                column: "ScheduleID");

            migrationBuilder.AddForeignKey(
                name: "FK_Displays_Schedules_ScheduleID",
                table: "Displays",
                column: "ScheduleID",
                principalTable: "Schedules",
                principalColumn: "ScheduleID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreDevices_Displays_DisplayID",
                table: "StoreDevices",
                column: "DisplayID",
                principalTable: "Displays",
                principalColumn: "DisplayID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
