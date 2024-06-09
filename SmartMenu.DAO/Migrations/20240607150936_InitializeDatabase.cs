using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class InitializeDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    BrandID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrandDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrandImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrandContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.BrandID);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    CollectionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandID = table.Column<int>(type: "int", nullable: false),
                    CollectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CollectionDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.CollectionID);
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
                name: "Menus",
                columns: table => new
                {
                    MenuID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandID = table.Column<int>(type: "int", nullable: false),
                    MenuName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MenuDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.MenuID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    StoreID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandID = table.Column<int>(type: "int", nullable: false),
                    StoreLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoreContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoreContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.StoreID);
                    table.ForeignKey(
                        name: "FK_Stores_Brands_BrandID",
                        column: x => x.BrandID,
                        principalTable: "Brands",
                        principalColumn: "BrandID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    TemplateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandID = table.Column<int>(type: "int", nullable: false),
                    TemplateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateWidth = table.Column<float>(type: "real", nullable: false),
                    TemplateHeight = table.Column<float>(type: "real", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.TemplateID);
                    table.ForeignKey(
                        name: "FK_Templates_Brands_BrandID",
                        column: x => x.BrandID,
                        principalTable: "Brands",
                        principalColumn: "BrandID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandID = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductPrice = table.Column<double>(type: "float", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK_Products_Brands_BrandID",
                        column: x => x.BrandID,
                        principalTable: "Brands",
                        principalColumn: "BrandID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductGroups",
                columns: table => new
                {
                    ProductGroupID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuID = table.Column<int>(type: "int", nullable: true),
                    CollectionID = table.Column<int>(type: "int", nullable: true),
                    ProductGroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductGroups", x => x.ProductGroupID);
                    table.ForeignKey(
                        name: "FK_ProductGroups_Collections_CollectionID",
                        column: x => x.CollectionID,
                        principalTable: "Collections",
                        principalColumn: "CollectionID");
                    table.ForeignKey(
                        name: "FK_ProductGroups_Menus_MenuID",
                        column: x => x.MenuID,
                        principalTable: "Menus",
                        principalColumn: "MenuID");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreCollections",
                columns: table => new
                {
                    StoreCollectionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreID = table.Column<int>(type: "int", nullable: false),
                    CollectionID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreCollections", x => x.StoreCollectionID);
                    table.ForeignKey(
                        name: "FK_StoreCollections_Collections_CollectionID",
                        column: x => x.CollectionID,
                        principalTable: "Collections",
                        principalColumn: "CollectionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreCollections_Stores_StoreID",
                        column: x => x.StoreID,
                        principalTable: "Stores",
                        principalColumn: "StoreID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreDevices",
                columns: table => new
                {
                    StoreDeviceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreID = table.Column<int>(type: "int", nullable: false),
                    StoreDeviceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayType = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreDevices", x => x.StoreDeviceID);
                    table.ForeignKey(
                        name: "FK_StoreDevices_Stores_StoreID",
                        column: x => x.StoreID,
                        principalTable: "Stores",
                        principalColumn: "StoreID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreMenus",
                columns: table => new
                {
                    StoreMenuID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreID = table.Column<int>(type: "int", nullable: false),
                    MenuID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreMenus", x => x.StoreMenuID);
                    table.ForeignKey(
                        name: "FK_StoreMenus_Menus_MenuID",
                        column: x => x.MenuID,
                        principalTable: "Menus",
                        principalColumn: "MenuID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreMenus_Stores_StoreID",
                        column: x => x.StoreID,
                        principalTable: "Stores",
                        principalColumn: "StoreID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Layers",
                columns: table => new
                {
                    LayerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateID = table.Column<int>(type: "int", nullable: false),
                    LayerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LayerType = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Layers", x => x.LayerID);
                    table.ForeignKey(
                        name: "FK_Layers_Templates_TemplateID",
                        column: x => x.TemplateID,
                        principalTable: "Templates",
                        principalColumn: "TemplateID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductGroupsItem",
                columns: table => new
                {
                    ProductGroupItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductGroupID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductGroupsItem", x => x.ProductGroupItemID);
                    table.ForeignKey(
                        name: "FK_ProductGroupsItem_ProductGroups_ProductGroupID",
                        column: x => x.ProductGroupID,
                        principalTable: "ProductGroups",
                        principalColumn: "ProductGroupID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductGroupsItem_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrandStaffs",
                columns: table => new
                {
                    BrandStaffID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandStaffs", x => x.BrandStaffID);
                    table.ForeignKey(
                        name: "FK_BrandStaffs_Brands_BrandID",
                        column: x => x.BrandID,
                        principalTable: "Brands",
                        principalColumn: "BrandID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandStaffs_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Displays",
                columns: table => new
                {
                    DisplayID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreDeviceID = table.Column<int>(type: "int", nullable: false),
                    MenuID = table.Column<int>(type: "int", nullable: true),
                    CollectionID = table.Column<int>(type: "int", nullable: true),
                    TemplateID = table.Column<int>(type: "int", nullable: false),
                    StartingHour = table.Column<double>(type: "float", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Displays", x => x.DisplayID);
                    table.ForeignKey(
                        name: "FK_Displays_Collections_CollectionID",
                        column: x => x.CollectionID,
                        principalTable: "Collections",
                        principalColumn: "CollectionID");
                    table.ForeignKey(
                        name: "FK_Displays_Menus_MenuID",
                        column: x => x.MenuID,
                        principalTable: "Menus",
                        principalColumn: "MenuID");
                    table.ForeignKey(
                        name: "FK_Displays_StoreDevices_StoreDeviceID",
                        column: x => x.StoreDeviceID,
                        principalTable: "StoreDevices",
                        principalColumn: "StoreDeviceID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Displays_Templates_TemplateID",
                        column: x => x.TemplateID,
                        principalTable: "Templates",
                        principalColumn: "TemplateID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Boxes",
                columns: table => new
                {
                    BoxID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LayerID = table.Column<int>(type: "int", nullable: false),
                    BoxPositionX = table.Column<float>(type: "real", nullable: false),
                    BoxPositionY = table.Column<float>(type: "real", nullable: false),
                    BoxWidth = table.Column<float>(type: "real", nullable: false),
                    BoxHeight = table.Column<float>(type: "real", nullable: false),
                    BoxMaxCapacity = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boxes", x => x.BoxID);
                    table.ForeignKey(
                        name: "FK_Boxes_Layers_LayerID",
                        column: x => x.LayerID,
                        principalTable: "Layers",
                        principalColumn: "LayerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LayersItem",
                columns: table => new
                {
                    LayerItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LayerID = table.Column<int>(type: "int", nullable: false),
                    LayerItemValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LayersItem", x => x.LayerItemID);
                    table.ForeignKey(
                        name: "FK_LayersItem_Layers_LayerID",
                        column: x => x.LayerID,
                        principalTable: "Layers",
                        principalColumn: "LayerID",
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

            migrationBuilder.CreateIndex(
                name: "IX_Boxes_LayerID",
                table: "Boxes",
                column: "LayerID");

            migrationBuilder.CreateIndex(
                name: "IX_BrandStaffs_BrandID",
                table: "BrandStaffs",
                column: "BrandID");

            migrationBuilder.CreateIndex(
                name: "IX_BrandStaffs_UserID",
                table: "BrandStaffs",
                column: "UserID");

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
                name: "IX_Displays_StoreDeviceID",
                table: "Displays",
                column: "StoreDeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_Displays_TemplateID",
                table: "Displays",
                column: "TemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_Layers_TemplateID",
                table: "Layers",
                column: "TemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_LayersItem_LayerID",
                table: "LayersItem",
                column: "LayerID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductGroups_CollectionID",
                table: "ProductGroups",
                column: "CollectionID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductGroups_MenuID",
                table: "ProductGroups",
                column: "MenuID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductGroupsItem_ProductGroupID",
                table: "ProductGroupsItem",
                column: "ProductGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductGroupsItem_ProductID",
                table: "ProductGroupsItem",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandID",
                table: "Products",
                column: "BrandID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryID",
                table: "Products",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_StoreCollections_CollectionID",
                table: "StoreCollections",
                column: "CollectionID");

            migrationBuilder.CreateIndex(
                name: "IX_StoreCollections_StoreID",
                table: "StoreCollections",
                column: "StoreID");

            migrationBuilder.CreateIndex(
                name: "IX_StoreDevices_StoreID",
                table: "StoreDevices",
                column: "StoreID");

            migrationBuilder.CreateIndex(
                name: "IX_StoreMenus_MenuID",
                table: "StoreMenus",
                column: "MenuID");

            migrationBuilder.CreateIndex(
                name: "IX_StoreMenus_StoreID",
                table: "StoreMenus",
                column: "StoreID");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_BrandID",
                table: "Stores",
                column: "BrandID");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_BrandID",
                table: "Templates",
                column: "BrandID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleID",
                table: "Users",
                column: "RoleID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandStaffs");

            migrationBuilder.DropTable(
                name: "DisplayItem");

            migrationBuilder.DropTable(
                name: "Fonts");

            migrationBuilder.DropTable(
                name: "LayersItem");

            migrationBuilder.DropTable(
                name: "ProductGroupsItem");

            migrationBuilder.DropTable(
                name: "StoreCollections");

            migrationBuilder.DropTable(
                name: "StoreMenus");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Boxes");

            migrationBuilder.DropTable(
                name: "Displays");

            migrationBuilder.DropTable(
                name: "ProductGroups");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Layers");

            migrationBuilder.DropTable(
                name: "StoreDevices");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "Menus");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Templates");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "Brands");
        }
    }
}
