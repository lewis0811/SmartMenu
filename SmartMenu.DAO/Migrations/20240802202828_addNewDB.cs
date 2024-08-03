using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    public partial class addNewDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    BrandId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrandDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrandImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrandContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.BrandId);
                });

            migrationBuilder.CreateTable(
                name: "Fonts",
                columns: table => new
                {
                    BFontId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FontName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FontPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fonts", x => x.BFontId);
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    TemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandId = table.Column<int>(type: "int", nullable: false),
                    TemplateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemplateWidth = table.Column<int>(type: "int", nullable: false),
                    TemplateHeight = table.Column<int>(type: "int", nullable: false),
                    TemplateType = table.Column<int>(type: "int", nullable: false),
                    TemplateImgPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.TemplateId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandId = table.Column<int>(type: "int", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                    table.ForeignKey(
                        name: "FK_Categories_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "BrandId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    CollectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandId = table.Column<int>(type: "int", nullable: false),
                    CollectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CollectionDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CollectionBackgroundImgPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.CollectionId);
                    table.ForeignKey(
                        name: "FK_Collections_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "BrandId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    MenuId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandId = table.Column<int>(type: "int", nullable: false),
                    MenuName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MenuDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.MenuId);
                    table.ForeignKey(
                        name: "FK_Menus_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "BrandId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    StoreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandId = table.Column<int>(type: "int", nullable: false),
                    StoreCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoreName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoreLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoreContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoreContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoreStatus = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.StoreId);
                    table.ForeignKey(
                        name: "FK_Stores_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "BrandId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Layers",
                columns: table => new
                {
                    LayerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    LayerType = table.Column<int>(type: "int", nullable: false),
                    ZIndex = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Layers", x => x.LayerId);
                    table.ForeignKey(
                        name: "FK_Layers_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductImgPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductLogoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductGroups",
                columns: table => new
                {
                    ProductGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuId = table.Column<int>(type: "int", nullable: true),
                    CollectionId = table.Column<int>(type: "int", nullable: true),
                    ProductGroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HaveNormalPrice = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductGroups", x => x.ProductGroupId);
                    table.ForeignKey(
                        name: "FK_ProductGroups_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "CollectionId");
                    table.ForeignKey(
                        name: "FK_ProductGroups_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "MenuId");
                });

            migrationBuilder.CreateTable(
                name: "BrandStaffs",
                columns: table => new
                {
                    BrandStaffId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandStaffs", x => x.BrandStaffId);
                    table.ForeignKey(
                        name: "FK_BrandStaffs_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "BrandId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandStaffs_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId");
                    table.ForeignKey(
                        name: "FK_BrandStaffs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreCollections",
                columns: table => new
                {
                    StoreCollectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreCollections", x => x.StoreCollectionId);
                    table.ForeignKey(
                        name: "FK_StoreCollections_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "CollectionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreCollections_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "StoreDevices",
                columns: table => new
                {
                    StoreDeviceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    StoreDeviceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeviceWidth = table.Column<float>(type: "real", nullable: false),
                    DeviceHeight = table.Column<float>(type: "real", nullable: false),
                    RatioType = table.Column<int>(type: "int", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreDevices", x => x.StoreDeviceId);
                    table.ForeignKey(
                        name: "FK_StoreDevices_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreMenus",
                columns: table => new
                {
                    StoreMenuId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    MenuId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreMenus", x => x.StoreMenuId);
                    table.ForeignKey(
                        name: "FK_StoreMenus_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "MenuId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreMenus_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Boxes",
                columns: table => new
                {
                    BoxId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LayerId = table.Column<int>(type: "int", nullable: false),
                    BoxPositionX = table.Column<float>(type: "real", nullable: false),
                    BoxPositionY = table.Column<float>(type: "real", nullable: false),
                    BoxWidth = table.Column<float>(type: "real", nullable: false),
                    BoxHeight = table.Column<float>(type: "real", nullable: false),
                    BoxType = table.Column<int>(type: "int", nullable: false),
                    MaxProductItem = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boxes", x => x.BoxId);
                    table.ForeignKey(
                        name: "FK_Boxes_Layers_LayerId",
                        column: x => x.LayerId,
                        principalTable: "Layers",
                        principalColumn: "LayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LayersItem",
                columns: table => new
                {
                    LayerItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LayerId = table.Column<int>(type: "int", nullable: false),
                    LayerItemValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LayersItem", x => x.LayerItemId);
                    table.ForeignKey(
                        name: "FK_LayersItem_Layers_LayerId",
                        column: x => x.LayerId,
                        principalTable: "Layers",
                        principalColumn: "LayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductSizePrices",
                columns: table => new
                {
                    ProductSizePriceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductSizeType = table.Column<int>(type: "int", nullable: false),
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
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreProducts",
                columns: table => new
                {
                    StoreProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreProducts", x => x.StoreProductId);
                    table.ForeignKey(
                        name: "FK_StoreProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreProducts_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ProductGroupsItem",
                columns: table => new
                {
                    ProductGroupItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductGroupId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductGroupsItem", x => x.ProductGroupItemId);
                    table.ForeignKey(
                        name: "FK_ProductGroupsItem_ProductGroups_ProductGroupId",
                        column: x => x.ProductGroupId,
                        principalTable: "ProductGroups",
                        principalColumn: "ProductGroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductGroupsItem_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Displays",
                columns: table => new
                {
                    DisplayId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreDeviceId = table.Column<int>(type: "int", nullable: false),
                    MenuId = table.Column<int>(type: "int", nullable: true),
                    CollectionId = table.Column<int>(type: "int", nullable: true),
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    ActiveHour = table.Column<double>(type: "float", nullable: false),
                    DisplayImgPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Displays", x => x.DisplayId);
                    table.ForeignKey(
                        name: "FK_Displays_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "CollectionId");
                    table.ForeignKey(
                        name: "FK_Displays_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "MenuId");
                    table.ForeignKey(
                        name: "FK_Displays_StoreDevices_StoreDeviceId",
                        column: x => x.StoreDeviceId,
                        principalTable: "StoreDevices",
                        principalColumn: "StoreDeviceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Displays_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoxItems",
                columns: table => new
                {
                    BoxItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoxId = table.Column<int>(type: "int", nullable: false),
                    BFontId = table.Column<int>(type: "int", nullable: true),
                    BoxItemX = table.Column<float>(type: "real", nullable: false),
                    BoxItemY = table.Column<float>(type: "real", nullable: false),
                    BoxItemWidth = table.Column<float>(type: "real", nullable: false),
                    BoxItemHeight = table.Column<float>(type: "real", nullable: false),
                    BoxItemType = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Style = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxItems", x => x.BoxItemId);
                    table.ForeignKey(
                        name: "FK_BoxItems_Boxes_BoxId",
                        column: x => x.BoxId,
                        principalTable: "Boxes",
                        principalColumn: "BoxId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoxItems_Fonts_BFontId",
                        column: x => x.BFontId,
                        principalTable: "Fonts",
                        principalColumn: "BFontId");
                });

            migrationBuilder.CreateTable(
                name: "DisplayItems",
                columns: table => new
                {
                    DisplayItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayId = table.Column<int>(type: "int", nullable: false),
                    BoxId = table.Column<int>(type: "int", nullable: false),
                    ProductGroupId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisplayItems", x => x.DisplayItemId);
                    table.ForeignKey(
                        name: "FK_DisplayItems_Boxes_BoxId",
                        column: x => x.BoxId,
                        principalTable: "Boxes",
                        principalColumn: "BoxId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DisplayItems_Displays_DisplayId",
                        column: x => x.DisplayId,
                        principalTable: "Displays",
                        principalColumn: "DisplayId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_DisplayItems_ProductGroups_ProductGroupId",
                        column: x => x.ProductGroupId,
                        principalTable: "ProductGroups",
                        principalColumn: "ProductGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boxes_LayerId",
                table: "Boxes",
                column: "LayerId");

            migrationBuilder.CreateIndex(
                name: "IX_BoxItems_BFontId",
                table: "BoxItems",
                column: "BFontId");

            migrationBuilder.CreateIndex(
                name: "IX_BoxItems_BoxId",
                table: "BoxItems",
                column: "BoxId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandStaffs_BrandId",
                table: "BrandStaffs",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandStaffs_StoreId",
                table: "BrandStaffs",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandStaffs_UserId",
                table: "BrandStaffs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_BrandId",
                table: "Categories",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_BrandId",
                table: "Collections",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_DisplayItems_BoxId",
                table: "DisplayItems",
                column: "BoxId");

            migrationBuilder.CreateIndex(
                name: "IX_DisplayItems_DisplayId",
                table: "DisplayItems",
                column: "DisplayId");

            migrationBuilder.CreateIndex(
                name: "IX_DisplayItems_ProductGroupId",
                table: "DisplayItems",
                column: "ProductGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Displays_CollectionId",
                table: "Displays",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Displays_MenuId",
                table: "Displays",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_Displays_StoreDeviceId",
                table: "Displays",
                column: "StoreDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Displays_TemplateId",
                table: "Displays",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Layers_TemplateId",
                table: "Layers",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_LayersItem_LayerId",
                table: "LayersItem",
                column: "LayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Menus_BrandId",
                table: "Menus",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductGroups_CollectionId",
                table: "ProductGroups",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductGroups_MenuId",
                table: "ProductGroups",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductGroupsItem_ProductGroupId",
                table: "ProductGroupsItem",
                column: "ProductGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductGroupsItem_ProductId",
                table: "ProductGroupsItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizePrices_ProductId",
                table: "ProductSizePrices",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreCollections_CollectionId",
                table: "StoreCollections",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreCollections_StoreId",
                table: "StoreCollections",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreDevices_StoreId",
                table: "StoreDevices",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreMenus_MenuId",
                table: "StoreMenus",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreMenus_StoreId",
                table: "StoreMenus",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProducts_ProductId",
                table: "StoreProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProducts_StoreId",
                table: "StoreProducts",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_BrandId",
                table: "Stores",
                column: "BrandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoxItems");

            migrationBuilder.DropTable(
                name: "BrandStaffs");

            migrationBuilder.DropTable(
                name: "DisplayItems");

            migrationBuilder.DropTable(
                name: "LayersItem");

            migrationBuilder.DropTable(
                name: "ProductGroupsItem");

            migrationBuilder.DropTable(
                name: "ProductSizePrices");

            migrationBuilder.DropTable(
                name: "StoreCollections");

            migrationBuilder.DropTable(
                name: "StoreMenus");

            migrationBuilder.DropTable(
                name: "StoreProducts");

            migrationBuilder.DropTable(
                name: "Fonts");

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
