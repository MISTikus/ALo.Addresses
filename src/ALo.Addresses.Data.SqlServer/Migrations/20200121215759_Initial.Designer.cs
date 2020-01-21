﻿// <auto-generated />
using System;
using ALo.Addresses.Data.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ALo.Addresses.Data.SqlServer.Migrations
{
    [DbContext(typeof(SqlServerFiasContext))]
    [Migration("20200121215759_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ALo.Addresses.Data.Models.Address", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte>("ActualityStatus")
                        .HasColumnType("tinyint");

                    b.Property<Guid>("AddressId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte>("DivisionType")
                        .HasColumnType("tinyint");

                    b.Property<byte>("Level")
                        .HasColumnType("tinyint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(120)")
                        .HasMaxLength(120);

                    b.Property<Guid?>("ParentAddressId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TypeShortName")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.HasIndex("AddressId")
                        .HasAnnotation("SqlServer:Include", new[] { "ActualityStatus" });

                    b.HasIndex("ParentAddressId")
                        .HasAnnotation("SqlServer:Include", new[] { "ActualityStatus" });

                    b.ToTable("Addresses","dbo");
                });

            modelBuilder.Entity("ALo.Addresses.Data.Models.House", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AddressId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BuildNumber")
                        .HasColumnType("nvarchar(10)")
                        .HasMaxLength(10);

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("HouseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("HouseNumber")
                        .HasColumnType("nvarchar(20)")
                        .HasMaxLength(20);

                    b.Property<byte>("HouseState")
                        .HasColumnType("tinyint");

                    b.Property<byte>("HouseType")
                        .HasColumnType("tinyint");

                    b.Property<string>("StructureNumber")
                        .HasColumnType("nvarchar(10)")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.HasIndex("AddressId")
                        .HasAnnotation("SqlServer:Include", new[] { "EndDate" });

                    b.HasIndex("HouseId")
                        .HasAnnotation("SqlServer:Include", new[] { "EndDate" });

                    b.ToTable("Houses","dbo");
                });
#pragma warning restore 612, 618
        }
    }
}
