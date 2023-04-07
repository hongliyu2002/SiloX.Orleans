﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vending.Projection.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    MoneyInfoInside_Yuan1 = table.Column<int>(type: "int", nullable: false),
                    MoneyInfoInside_Yuan2 = table.Column<int>(type: "int", nullable: false),
                    MoneyInfoInside_Yuan5 = table.Column<int>(type: "int", nullable: false),
                    MoneyInfoInside_Yuan10 = table.Column<int>(type: "int", nullable: false),
                    MoneyInfoInside_Yuan20 = table.Column<int>(type: "int", nullable: false),
                    MoneyInfoInside_Yuan50 = table.Column<int>(type: "int", nullable: false),
                    MoneyInfoInside_Yuan100 = table.Column<int>(type: "int", nullable: false),
                    MoneyInfoInside_Amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    AmountInTransaction = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LastModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    SlotsCount = table.Column<int>(type: "int", nullable: false),
                    SnackCount = table.Column<int>(type: "int", nullable: false),
                    SnackQuantity = table.Column<int>(type: "int", nullable: false),
                    SnackAmount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    BoughtCount = table.Column<int>(type: "int", nullable: false),
                    BoughtAmount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Snacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    PictureUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LastModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    MachineCount = table.Column<int>(type: "int", nullable: false),
                    TotalQuantity = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    BoughtCount = table.Column<int>(type: "int", nullable: false),
                    BoughtAmount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MachineSlots",
                columns: table => new
                {
                    MachineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    SnackPile_SnackId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SnackPile_SnackName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SnackPile_SnackPictureUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    SnackPile_Quantity = table.Column<int>(type: "int", nullable: true),
                    SnackPile_Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    SnackPile_Amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineSlots", x => new { x.MachineId, x.Position });
                    table.ForeignKey(
                        name: "FK_MachineSlots_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MachineSlots_Snacks_SnackPile_SnackId",
                        column: x => x.SnackPile_SnackId,
                        principalTable: "Snacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MachineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    SnackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SnackName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SnackPictureUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    BoughtPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    BoughtAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    BoughtBy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Purchases_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Purchases_Snacks_SnackId",
                        column: x => x.SnackId,
                        principalTable: "Snacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Machines_IsDeleted_CreatedAt",
                table: "Machines",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Machines_IsDeleted_LastModifiedAt",
                table: "Machines",
                columns: new[] { "IsDeleted", "LastModifiedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MachineSlots_SnackPile_SnackId",
                table: "MachineSlots",
                column: "SnackPile_SnackId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_MachineId",
                table: "Purchases",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_SnackId",
                table: "Purchases",
                column: "SnackId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_SnackName",
                table: "Purchases",
                column: "SnackName");

            migrationBuilder.CreateIndex(
                name: "IX_Snacks_IsDeleted_CreatedAt",
                table: "Snacks",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Snacks_IsDeleted_LastModifiedAt",
                table: "Snacks",
                columns: new[] { "IsDeleted", "LastModifiedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Snacks_IsDeleted_Name",
                table: "Snacks",
                columns: new[] { "IsDeleted", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MachineSlots");

            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropTable(
                name: "Machines");

            migrationBuilder.DropTable(
                name: "Snacks");
        }
    }
}
