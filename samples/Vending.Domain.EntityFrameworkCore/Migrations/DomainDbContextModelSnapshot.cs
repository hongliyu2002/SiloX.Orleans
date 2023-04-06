﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vending.Domain.EntityFrameworkCore;

#nullable disable

namespace Vending.Domain.EntityFrameworkCore.Migrations
{
    [DbContext(typeof(DomainDbContext))]
    partial class DomainDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Vending.Domain.Abstractions.Purchases.Purchase", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset?>("BoughtAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("BoughtBy")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<decimal>("BoughtPrice")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<Guid>("MachineId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Position")
                        .HasColumnType("int");

                    b.Property<Guid>("SnackId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("MachineId");

                    b.HasIndex("SnackId");

                    b.ToTable("Purchases", (string)null);
                });

            modelBuilder.Entity("Vending.Domain.Abstractions.SnackMachines.Slot", b =>
                {
                    b.Property<Guid>("MachineId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Position")
                        .HasColumnType("int");

                    b.HasKey("MachineId", "Position");

                    b.ToTable("Slots", (string)null);
                });

            modelBuilder.Entity("Vending.Domain.Abstractions.SnackMachines.SnackMachine", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("AmountInTransaction")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<DateTimeOffset?>("CreatedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("DeletedBy")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LastModifiedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("LastModifiedBy")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("SlotsCount")
                        .HasColumnType("int");

                    b.Property<decimal>("SnackAmount")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("SnackCount")
                        .HasColumnType("int");

                    b.Property<int>("SnackQuantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("SnackMachines", (string)null);
                });

            modelBuilder.Entity("Vending.Domain.Abstractions.SnackMachines.SnackStat", b =>
                {
                    b.Property<Guid>("MachineId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SnackId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("TotalAmount")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("TotalQuantity")
                        .HasColumnType("int");

                    b.HasKey("MachineId", "SnackId");

                    b.HasIndex("SnackId");

                    b.ToTable("SnackStat");
                });

            modelBuilder.Entity("Vending.Domain.Abstractions.Snacks.Snack", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset?>("CreatedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("DeletedBy")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LastModifiedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("LastModifiedBy")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("PictureUrl")
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.HasKey("Id");

                    b.HasIndex("IsDeleted", "Name");

                    b.ToTable("Snacks", (string)null);
                });

            modelBuilder.Entity("Vending.Domain.Abstractions.Purchases.Purchase", b =>
                {
                    b.HasOne("Vending.Domain.Abstractions.SnackMachines.SnackMachine", null)
                        .WithMany()
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Vending.Domain.Abstractions.Snacks.Snack", null)
                        .WithMany()
                        .HasForeignKey("SnackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Vending.Domain.Abstractions.SnackMachines.Slot", b =>
                {
                    b.HasOne("Vending.Domain.Abstractions.SnackMachines.SnackMachine", null)
                        .WithMany("Slots")
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Vending.Domain.Abstractions.SnackMachines.SnackPile", "SnackPile", b1 =>
                        {
                            b1.Property<Guid>("SlotMachineId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("SlotPosition")
                                .HasColumnType("int");

                            b1.Property<decimal>("Amount")
                                .HasPrecision(10, 2)
                                .HasColumnType("decimal(10,2)");

                            b1.Property<decimal>("Price")
                                .HasPrecision(10, 2)
                                .HasColumnType("decimal(10,2)");

                            b1.Property<int>("Quantity")
                                .HasColumnType("int");

                            b1.Property<Guid>("SnackId")
                                .HasColumnType("uniqueidentifier");

                            b1.HasKey("SlotMachineId", "SlotPosition");

                            b1.HasIndex("SnackId");

                            b1.ToTable("Slots");

                            b1.HasOne("Vending.Domain.Abstractions.Snacks.Snack", null)
                                .WithMany()
                                .HasForeignKey("SnackId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b1.WithOwner()
                                .HasForeignKey("SlotMachineId", "SlotPosition");
                        });

                    b.Navigation("SnackPile");
                });

            modelBuilder.Entity("Vending.Domain.Abstractions.SnackMachines.SnackMachine", b =>
                {
                    b.OwnsOne("Vending.Domain.Abstractions.SnackMachines.Money", "MoneyInside", b1 =>
                        {
                            b1.Property<Guid>("SnackMachineId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<decimal>("Amount")
                                .HasPrecision(10, 2)
                                .HasColumnType("decimal(10,2)");

                            b1.Property<int>("Yuan1")
                                .HasColumnType("int");

                            b1.Property<int>("Yuan10")
                                .HasColumnType("int");

                            b1.Property<int>("Yuan100")
                                .HasColumnType("int");

                            b1.Property<int>("Yuan2")
                                .HasColumnType("int");

                            b1.Property<int>("Yuan20")
                                .HasColumnType("int");

                            b1.Property<int>("Yuan5")
                                .HasColumnType("int");

                            b1.Property<int>("Yuan50")
                                .HasColumnType("int");

                            b1.HasKey("SnackMachineId");

                            b1.ToTable("SnackMachines");

                            b1.WithOwner()
                                .HasForeignKey("SnackMachineId");
                        });

                    b.Navigation("MoneyInside")
                        .IsRequired();
                });

            modelBuilder.Entity("Vending.Domain.Abstractions.SnackMachines.SnackStat", b =>
                {
                    b.HasOne("Vending.Domain.Abstractions.SnackMachines.SnackMachine", null)
                        .WithMany("SnackStats")
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Vending.Domain.Abstractions.Snacks.Snack", null)
                        .WithMany()
                        .HasForeignKey("SnackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Vending.Domain.Abstractions.SnackMachines.SnackMachine", b =>
                {
                    b.Navigation("Slots");

                    b.Navigation("SnackStats");
                });
#pragma warning restore 612, 618
        }
    }
}
