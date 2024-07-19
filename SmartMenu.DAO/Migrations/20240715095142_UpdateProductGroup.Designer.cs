﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartMenu.DAO;

#nullable disable

namespace SmartMenu.DAO.Migrations
{
    [DbContext(typeof(SmartMenuDBContext))]
    [Migration("20240715095142_UpdateProductGroup")]
    partial class UpdateProductGroup
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.29")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("SmartMenu.Domain.Models.Box", b =>
                {
                    b.Property<int>("BoxId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BoxId"), 1L, 1);

                    b.Property<float>("BoxHeight")
                        .HasColumnType("real");

                    b.Property<int>("BoxMaxCapacity")
                        .HasColumnType("int");

                    b.Property<float>("BoxPositionX")
                        .HasColumnType("real");

                    b.Property<float>("BoxPositionY")
                        .HasColumnType("real");

                    b.Property<int>("BoxType")
                        .HasColumnType("int");

                    b.Property<float>("BoxWidth")
                        .HasColumnType("real");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("LayerId")
                        .HasColumnType("int");

                    b.HasKey("BoxId");

                    b.HasIndex("LayerId");

                    b.ToTable("Boxes");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.BoxItem", b =>
                {
                    b.Property<int>("BoxItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BoxItemId"), 1L, 1);

                    b.Property<string>("BoxColor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("BoxId")
                        .HasColumnType("int");

                    b.Property<int>("BoxItemType")
                        .HasColumnType("int");

                    b.Property<int>("FontId")
                        .HasColumnType("int");

                    b.Property<double>("FontSize")
                        .HasColumnType("float");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("TextFormat")
                        .HasColumnType("int");

                    b.HasKey("BoxItemId");

                    b.HasIndex("BoxId");

                    b.HasIndex("FontId");

                    b.ToTable("BoxItems");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Brand", b =>
                {
                    b.Property<int>("BrandId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BrandId"), 1L, 1);

                    b.Property<string>("BrandContactEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BrandDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BrandImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BrandName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.HasKey("BrandId");

                    b.ToTable("Brands");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.BrandStaff", b =>
                {
                    b.Property<int>("BrandStaffId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BrandStaffId"), 1L, 1);

                    b.Property<int>("BrandId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int?>("StoreId")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("BrandStaffId");

                    b.HasIndex("BrandId");

                    b.HasIndex("StoreId");

                    b.HasIndex("UserId");

                    b.ToTable("BrandStaffs");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"), 1L, 1);

                    b.Property<int>("BrandId")
                        .HasColumnType("int");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.HasKey("CategoryId");

                    b.HasIndex("BrandId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Collection", b =>
                {
                    b.Property<int>("CollectionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CollectionId"), 1L, 1);

                    b.Property<int>("BrandId")
                        .HasColumnType("int");

                    b.Property<string>("CollectionDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CollectionName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.HasKey("CollectionId");

                    b.ToTable("Collections");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Display", b =>
                {
                    b.Property<int>("DisplayId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DisplayId"), 1L, 1);

                    b.Property<double>("ActiveHour")
                        .HasColumnType("float");

                    b.Property<int?>("CollectionId")
                        .HasColumnType("int");

                    b.Property<string>("DisplayImgPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int?>("MenuId")
                        .HasColumnType("int");

                    b.Property<int>("StoreDeviceId")
                        .HasColumnType("int");

                    b.Property<int>("TemplateId")
                        .HasColumnType("int");

                    b.HasKey("DisplayId");

                    b.HasIndex("CollectionId");

                    b.HasIndex("MenuId");

                    b.HasIndex("StoreDeviceId");

                    b.HasIndex("TemplateId");

                    b.ToTable("Displays");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.DisplayItem", b =>
                {
                    b.Property<int>("DisplayItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DisplayItemId"), 1L, 1);

                    b.Property<int>("BoxId")
                        .HasColumnType("int");

                    b.Property<int>("DisplayId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("ProductGroupId")
                        .HasColumnType("int");

                    b.HasKey("DisplayItemId");

                    b.HasIndex("BoxId");

                    b.HasIndex("DisplayId");

                    b.HasIndex("ProductGroupId");

                    b.ToTable("DisplayItems");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Font", b =>
                {
                    b.Property<int>("FontId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FontId"), 1L, 1);

                    b.Property<string>("FontName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FontPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.HasKey("FontId");

                    b.ToTable("Fonts");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Layer", b =>
                {
                    b.Property<int>("LayerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LayerId"), 1L, 1);

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("LayerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LayerType")
                        .HasColumnType("int");

                    b.Property<int>("TemplateId")
                        .HasColumnType("int");

                    b.HasKey("LayerId");

                    b.HasIndex("TemplateId");

                    b.ToTable("Layers");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.LayerItem", b =>
                {
                    b.Property<int>("LayerItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LayerItemId"), 1L, 1);

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("LayerId")
                        .HasColumnType("int");

                    b.Property<string>("LayerItemValue")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LayerItemId");

                    b.HasIndex("LayerId")
                        .IsUnique();

                    b.ToTable("LayersItem");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Menu", b =>
                {
                    b.Property<int>("MenuId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MenuId"), 1L, 1);

                    b.Property<int>("BrandId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("MenuDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MenuName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MenuId");

                    b.ToTable("Menus");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"), 1L, 1);

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("ProductDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProductId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.ProductGroup", b =>
                {
                    b.Property<int>("ProductGroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductGroupId"), 1L, 1);

                    b.Property<int?>("CollectionId")
                        .HasColumnType("int");

                    b.Property<bool>("HaveNormalPrice")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int?>("MenuId")
                        .HasColumnType("int");

                    b.Property<string>("ProductGroupName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProductGroupId");

                    b.HasIndex("CollectionId");

                    b.HasIndex("MenuId");

                    b.ToTable("ProductGroups");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.ProductGroupItem", b =>
                {
                    b.Property<int>("ProductGroupItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductGroupItemId"), 1L, 1);

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("ProductGroupId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("ProductGroupItemId");

                    b.HasIndex("ProductGroupId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductGroupsItem");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.ProductSize", b =>
                {
                    b.Property<int>("ProductSizeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductSizeId"), 1L, 1);

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("SizeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProductSizeId");

                    b.ToTable("ProductSizes");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.ProductSizePrice", b =>
                {
                    b.Property<int>("ProductSizePriceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductSizePriceId"), 1L, 1);

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("ProductSizeType")
                        .HasColumnType("int");

                    b.HasKey("ProductSizePriceId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductSizePrices");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Store", b =>
                {
                    b.Property<int>("StoreId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StoreId"), 1L, 1);

                    b.Property<int>("BrandId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("StoreContactEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StoreContactNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StoreLocation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StoreId");

                    b.HasIndex("BrandId");

                    b.ToTable("Stores");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.StoreCollection", b =>
                {
                    b.Property<int>("StoreCollectionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StoreCollectionId"), 1L, 1);

                    b.Property<int>("CollectionId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.HasKey("StoreCollectionId");

                    b.HasIndex("CollectionId");

                    b.HasIndex("StoreId");

                    b.ToTable("StoreCollections");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.StoreDevice", b =>
                {
                    b.Property<int>("StoreDeviceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StoreDeviceId"), 1L, 1);

                    b.Property<float>("DeviceHeight")
                        .HasColumnType("real");

                    b.Property<float>("DeviceWidth")
                        .HasColumnType("real");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("StoreDeviceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.HasKey("StoreDeviceId");

                    b.ToTable("StoreDevices");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.StoreMenu", b =>
                {
                    b.Property<int>("StoreMenuId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StoreMenuId"), 1L, 1);

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("MenuId")
                        .HasColumnType("int");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.HasKey("StoreMenuId");

                    b.HasIndex("MenuId");

                    b.HasIndex("StoreId");

                    b.ToTable("StoreMenus");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.StoreProduct", b =>
                {
                    b.Property<int>("StoreProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StoreProductId"), 1L, 1);

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.HasKey("StoreProductId");

                    b.HasIndex("ProductId");

                    b.HasIndex("StoreId");

                    b.ToTable("StoreProducts");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Template", b =>
                {
                    b.Property<int>("TemplateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TemplateId"), 1L, 1);

                    b.Property<int>("BrandId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("TemplateDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("TemplateHeight")
                        .HasColumnType("real");

                    b.Property<string>("TemplateImgPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TemplateName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("TemplateWidth")
                        .HasColumnType("real");

                    b.HasKey("TemplateId");

                    b.ToTable("Templates");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.User", b =>
                {
                    b.Property<Guid>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Box", b =>
                {
                    b.HasOne("SmartMenu.Domain.Models.Layer", null)
                        .WithMany("Boxes")
                        .HasForeignKey("LayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.BoxItem", b =>
                {
                    b.HasOne("SmartMenu.Domain.Models.Box", null)
                        .WithMany("BoxItems")
                        .HasForeignKey("BoxId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartMenu.Domain.Models.Font", "Font")
                        .WithMany()
                        .HasForeignKey("FontId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Font");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.BrandStaff", b =>
                {
                    b.HasOne("SmartMenu.Domain.Models.Brand", null)
                        .WithMany("BrandStaffs")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartMenu.Domain.Models.Store", null)
                        .WithMany("Staffs")
                        .HasForeignKey("StoreId");

                    b.HasOne("SmartMenu.Domain.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Category", b =>
                {
                    b.HasOne("SmartMenu.Domain.Models.Brand", null)
                        .WithMany("Categories")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Display", b =>
                {
                    b.HasOne("SmartMenu.Domain.Models.Collection", "Collection")
                        .WithMany()
                        .HasForeignKey("CollectionId");

                    b.HasOne("SmartMenu.Domain.Models.Menu", "Menu")
                        .WithMany()
                        .HasForeignKey("MenuId");

                    b.HasOne("SmartMenu.Domain.Models.StoreDevice", "StoreDevice")
                        .WithMany("Displays")
                        .HasForeignKey("StoreDeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartMenu.Domain.Models.Template", "Template")
                        .WithMany()
                        .HasForeignKey("TemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Collection");

                    b.Navigation("Menu");

                    b.Navigation("StoreDevice");

                    b.Navigation("Template");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.DisplayItem", b =>
                {
                    b.HasOne("SmartMenu.Domain.Models.Box", "Box")
                        .WithMany()
                        .HasForeignKey("BoxId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartMenu.Domain.Models.Display", null)
                        .WithMany("DisplayItems")
                        .HasForeignKey("DisplayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartMenu.Domain.Models.ProductGroup", "ProductGroup")
                        .WithMany()
                        .HasForeignKey("ProductGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Box");

                    b.Navigation("ProductGroup");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Layer", b =>
                {
                    b.HasOne("SmartMenu.Domain.Models.Template", null)
                        .WithMany("Layers")
                        .HasForeignKey("TemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.LayerItem", b =>
                {
                    b.HasOne("SmartMenu.Domain.Models.Layer", null)
                        .WithOne("LayerItem")
                        .HasForeignKey("SmartMenu.Domain.Models.LayerItem", "LayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Product", b =>
                {
                    b.HasOne("SmartMenu.Domain.Models.Category", null)
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.ProductGroup", b =>
                {
                    b.HasOne("SmartMenu.Domain.Models.Collection", "Collection")
                        .WithMany("ProductGroups")
                        .HasForeignKey("CollectionId");

                    b.HasOne("SmartMenu.Domain.Models.Menu", "Menu")
                        .WithMany("ProductGroups")
                        .HasForeignKey("MenuId");

                    b.Navigation("Collection");

                    b.Navigation("Menu");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.ProductGroupItem", b =>
                {
                    b.HasOne("SmartMenu.Domain.Models.ProductGroup", null)
                        .WithMany("ProductGroupItems")
                        .HasForeignKey("ProductGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartMenu.Domain.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.ProductSizePrice", b =>
                {
                    b.HasOne("SmartMenu.Domain.Models.Product", null)
                        .WithMany("ProductSizePrices")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Store", b =>
                {
                    b.HasOne("SmartMenu.Domain.Models.Brand", null)
                        .WithMany("Stores")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.StoreCollection", b =>
                {
                    b.HasOne("SmartMenu.Domain.Models.Collection", "Collection")
                        .WithMany()
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartMenu.Domain.Models.Store", null)
                        .WithMany("StoreCollections")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Collection");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.StoreMenu", b =>
                {
                    b.HasOne("SmartMenu.Domain.Models.Menu", "Menu")
                        .WithMany()
                        .HasForeignKey("MenuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartMenu.Domain.Models.Store", null)
                        .WithMany("StoreMenus")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Menu");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.StoreProduct", b =>
                {
                    b.HasOne("SmartMenu.Domain.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartMenu.Domain.Models.Store", "Store")
                        .WithMany()
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Store");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Box", b =>
                {
                    b.Navigation("BoxItems");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Brand", b =>
                {
                    b.Navigation("BrandStaffs");

                    b.Navigation("Categories");

                    b.Navigation("Stores");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Collection", b =>
                {
                    b.Navigation("ProductGroups");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Display", b =>
                {
                    b.Navigation("DisplayItems");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Layer", b =>
                {
                    b.Navigation("Boxes");

                    b.Navigation("LayerItem");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Menu", b =>
                {
                    b.Navigation("ProductGroups");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Product", b =>
                {
                    b.Navigation("ProductSizePrices");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.ProductGroup", b =>
                {
                    b.Navigation("ProductGroupItems");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Store", b =>
                {
                    b.Navigation("Staffs");

                    b.Navigation("StoreCollections");

                    b.Navigation("StoreMenus");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.StoreDevice", b =>
                {
                    b.Navigation("Displays");
                });

            modelBuilder.Entity("SmartMenu.Domain.Models.Template", b =>
                {
                    b.Navigation("Layers");
                });
#pragma warning restore 612, 618
        }
    }
}
