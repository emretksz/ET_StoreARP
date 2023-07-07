﻿// <auto-generated />
using System;
using DataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataAccess.Migrations
{
    [DbContext(typeof(ET_StoreARP))]
    [Migration("20221222105733_xds")]
    partial class xds
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("ColorProduct", b =>
                {
                    b.Property<long>("ColorsId")
                        .HasColumnType("bigint");

                    b.Property<long>("ProductsId")
                        .HasColumnType("bigint");

                    b.HasKey("ColorsId", "ProductsId");

                    b.HasIndex("ProductsId");

                    b.ToTable("ColorProduct");
                });

            modelBuilder.Entity("Entities.Concrete.AppRole", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("RoleName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AppRole");
                });

            modelBuilder.Entity("Entities.Concrete.AppUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<long?>("AppRoleId")
                        .HasColumnType("bigint");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.Property<long>("TenantId")
                        .HasColumnType("bigint");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AppRoleId");

                    b.HasIndex("TenantId");

                    b.ToTable("AppUsers");
                });

            modelBuilder.Entity("Entities.Concrete.Color", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("ColorName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<long?>("MagazaStockId")
                        .HasColumnType("bigint");

                    b.Property<long?>("OrderId")
                        .HasColumnType("bigint");

                    b.Property<long>("ShippingsId")
                        .HasColumnType("bigint");

                    b.Property<long?>("StockId")
                        .HasColumnType("bigint");

                    b.Property<long?>("TempId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("MagazaStockId");

                    b.HasIndex("OrderId");

                    b.HasIndex("StockId");

                    b.HasIndex("TempId");

                    b.ToTable("Colors");
                });

            modelBuilder.Entity("Entities.Concrete.MagazaStock", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<long>("ColorId")
                        .HasColumnType("bigint");

                    b.Property<long>("ProductId")
                        .HasColumnType("bigint");

                    b.Property<string>("RegisterDate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("StockCount")
                        .HasColumnType("bigint");

                    b.Property<int>("StockStatus")
                        .HasColumnType("int");

                    b.Property<long?>("TempId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("TempId");

                    b.ToTable("MagazaStocks");
                });

            modelBuilder.Entity("Entities.Concrete.Order", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<long>("ColorId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsFinised")
                        .HasColumnType("bit");

                    b.Property<long>("OrderDateId")
                        .HasColumnType("bigint");

                    b.Property<int>("OrdersStatus")
                        .HasColumnType("int");

                    b.Property<string>("ProductCount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("ProductId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("datetime2");

                    b.Property<long>("ShippingId")
                        .HasColumnType("bigint");

                    b.Property<long>("TenantId")
                        .HasColumnType("bigint");

                    b.Property<string>("TotalCount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TotalPrice")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Entities.Concrete.OrderDate", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("FaturaAdi")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("FaturaOlusturmaZamani")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("OrderDates");
                });

            modelBuilder.Entity("Entities.Concrete.Product", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<string>("Barcode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<string>("IsActive")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("MagazaStockId")
                        .HasColumnType("bigint");

                    b.Property<string>("ModelCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ModelColor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ModelCount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ModelImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ModelName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("OrderId")
                        .HasColumnType("bigint");

                    b.Property<long>("ProductAgesId")
                        .HasColumnType("bigint");

                    b.Property<string>("RegisterDate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("StockId")
                        .HasColumnType("bigint");

                    b.Property<long?>("TempId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("MagazaStockId");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductAgesId");

                    b.HasIndex("StockId");

                    b.HasIndex("TempId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Entities.Concrete.ProductAges", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ProductAges");
                });

            modelBuilder.Entity("Entities.Concrete.ShippingDetails", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<long>("Amount")
                        .HasColumnType("bigint");

                    b.Property<long?>("ColorId")
                        .HasColumnType("bigint");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<long>("ProductId")
                        .HasColumnType("bigint");

                    b.Property<long>("ShippinbgId")
                        .HasColumnType("bigint");

                    b.Property<long?>("ShippingCount")
                        .HasColumnType("bigint");

                    b.Property<long?>("ShippingsId")
                        .HasColumnType("bigint");

                    b.Property<bool>("SiparisTamamlandi")
                        .HasColumnType("bit");

                    b.Property<bool>("SiparislereEklendi")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("ColorId");

                    b.HasIndex("ProductId");

                    b.HasIndex("ShippingsId");

                    b.ToTable("ShippingDetails");
                });

            modelBuilder.Entity("Entities.Concrete.Shippings", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("Adres")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<long?>("ColorId")
                        .HasColumnType("bigint");

                    b.Property<bool>("DepoTamam")
                        .HasColumnType("bit");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsComplated")
                        .HasColumnType("bit");

                    b.Property<long?>("OrderId")
                        .HasColumnType("bigint");

                    b.Property<long?>("ProductId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ShippingCount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ShippingStasus")
                        .HasColumnType("int");

                    b.Property<string>("SiparisAdi")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SiparisTutari")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("TempId")
                        .HasColumnType("bigint");

                    b.Property<long>("TenantId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ColorId");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.HasIndex("TempId");

                    b.HasIndex("TenantId");

                    b.ToTable("Shippings");
                });

            modelBuilder.Entity("Entities.Concrete.Stock", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<long>("ColorId")
                        .HasColumnType("bigint");

                    b.Property<long?>("MagazaId")
                        .HasColumnType("bigint");

                    b.Property<bool?>("MagazaMi")
                        .HasColumnType("bit");

                    b.Property<long>("ProductId")
                        .HasColumnType("bigint");

                    b.Property<string>("RegisterDate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("StockCount")
                        .HasColumnType("bigint");

                    b.Property<int>("StockStatus")
                        .HasColumnType("int");

                    b.Property<long>("StockTemp")
                        .HasColumnType("bigint");

                    b.Property<bool?>("TekliMi")
                        .HasColumnType("bit");

                    b.Property<long?>("TempId")
                        .HasColumnType("bigint");

                    b.Property<long?>("tekliId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("TempId");

                    b.ToTable("Stocks");
                });

            modelBuilder.Entity("Entities.Concrete.Tekliler", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<long>("ColorId")
                        .HasColumnType("bigint");

                    b.Property<long>("Count")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<long>("ProductId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Teklilers");
                });

            modelBuilder.Entity("Entities.Concrete.Temp", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<long>("ColorId")
                        .HasColumnType("bigint");

                    b.Property<long>("Count")
                        .HasColumnType("bigint");

                    b.Property<int>("DepocuDurumu")
                        .HasColumnType("int");

                    b.Property<bool>("IsComplated")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsFinished")
                        .HasColumnType("bit");

                    b.Property<long>("OrderDateId")
                        .HasColumnType("bigint");

                    b.Property<long>("ProductId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("datetime2");

                    b.Property<long>("ShippigId")
                        .HasColumnType("bigint");

                    b.Property<long>("StockId")
                        .HasColumnType("bigint");

                    b.Property<long>("TenantId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Temps");
                });

            modelBuilder.Entity("Entities.Concrete.Tenant", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("RegisterDate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TenantName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Tenants");
                });

            modelBuilder.Entity("ColorProduct", b =>
                {
                    b.HasOne("Entities.Concrete.Color", null)
                        .WithMany()
                        .HasForeignKey("ColorsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entities.Concrete.Product", null)
                        .WithMany()
                        .HasForeignKey("ProductsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Entities.Concrete.AppUser", b =>
                {
                    b.HasOne("Entities.Concrete.AppRole", "AppRole")
                        .WithMany("AppUser")
                        .HasForeignKey("AppRoleId");

                    b.HasOne("Entities.Concrete.Tenant", "Tenant")
                        .WithMany("AppUsers")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppRole");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("Entities.Concrete.Color", b =>
                {
                    b.HasOne("Entities.Concrete.MagazaStock", null)
                        .WithMany("Colors")
                        .HasForeignKey("MagazaStockId");

                    b.HasOne("Entities.Concrete.Order", null)
                        .WithMany("Color")
                        .HasForeignKey("OrderId");

                    b.HasOne("Entities.Concrete.Stock", "Stock")
                        .WithMany("Colors")
                        .HasForeignKey("StockId");

                    b.HasOne("Entities.Concrete.Temp", null)
                        .WithMany("Colors")
                        .HasForeignKey("TempId");

                    b.Navigation("Stock");
                });

            modelBuilder.Entity("Entities.Concrete.MagazaStock", b =>
                {
                    b.HasOne("Entities.Concrete.Temp", "Temp")
                        .WithMany()
                        .HasForeignKey("TempId");

                    b.Navigation("Temp");
                });

            modelBuilder.Entity("Entities.Concrete.Product", b =>
                {
                    b.HasOne("Entities.Concrete.MagazaStock", null)
                        .WithMany("Product")
                        .HasForeignKey("MagazaStockId");

                    b.HasOne("Entities.Concrete.Order", null)
                        .WithMany("Products")
                        .HasForeignKey("OrderId");

                    b.HasOne("Entities.Concrete.ProductAges", "ProductAges")
                        .WithMany()
                        .HasForeignKey("ProductAgesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entities.Concrete.Stock", "Stock")
                        .WithMany("Product")
                        .HasForeignKey("StockId");

                    b.HasOne("Entities.Concrete.Temp", null)
                        .WithMany("Products")
                        .HasForeignKey("TempId");

                    b.Navigation("ProductAges");

                    b.Navigation("Stock");
                });

            modelBuilder.Entity("Entities.Concrete.ShippingDetails", b =>
                {
                    b.HasOne("Entities.Concrete.Color", "Color")
                        .WithMany()
                        .HasForeignKey("ColorId");

                    b.HasOne("Entities.Concrete.Product", "Products")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entities.Concrete.Shippings", "Shippings")
                        .WithMany()
                        .HasForeignKey("ShippingsId");

                    b.Navigation("Color");

                    b.Navigation("Products");

                    b.Navigation("Shippings");
                });

            modelBuilder.Entity("Entities.Concrete.Shippings", b =>
                {
                    b.HasOne("Entities.Concrete.Color", null)
                        .WithMany("Shippings")
                        .HasForeignKey("ColorId");

                    b.HasOne("Entities.Concrete.Order", null)
                        .WithMany("Shippings")
                        .HasForeignKey("OrderId");

                    b.HasOne("Entities.Concrete.Product", null)
                        .WithMany("Shippings")
                        .HasForeignKey("ProductId");

                    b.HasOne("Entities.Concrete.Temp", null)
                        .WithMany("Shippings")
                        .HasForeignKey("TempId");

                    b.HasOne("Entities.Concrete.Tenant", "Tenant")
                        .WithMany("Shippings")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("Entities.Concrete.Stock", b =>
                {
                    b.HasOne("Entities.Concrete.Temp", "Temp")
                        .WithMany("Stock")
                        .HasForeignKey("TempId");

                    b.Navigation("Temp");
                });

            modelBuilder.Entity("Entities.Concrete.AppRole", b =>
                {
                    b.Navigation("AppUser");
                });

            modelBuilder.Entity("Entities.Concrete.Color", b =>
                {
                    b.Navigation("Shippings");
                });

            modelBuilder.Entity("Entities.Concrete.MagazaStock", b =>
                {
                    b.Navigation("Colors");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Entities.Concrete.Order", b =>
                {
                    b.Navigation("Color");

                    b.Navigation("Products");

                    b.Navigation("Shippings");
                });

            modelBuilder.Entity("Entities.Concrete.Product", b =>
                {
                    b.Navigation("Shippings");
                });

            modelBuilder.Entity("Entities.Concrete.Stock", b =>
                {
                    b.Navigation("Colors");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Entities.Concrete.Temp", b =>
                {
                    b.Navigation("Colors");

                    b.Navigation("Products");

                    b.Navigation("Shippings");

                    b.Navigation("Stock");
                });

            modelBuilder.Entity("Entities.Concrete.Tenant", b =>
                {
                    b.Navigation("AppUsers");

                    b.Navigation("Shippings");
                });
#pragma warning restore 612, 618
        }
    }
}
