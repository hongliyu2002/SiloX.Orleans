using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vending.Domain.EntityFrameworkCore.Migrations
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
                    MoneyInside_Yuan1 = table.Column<int>(type: "int", nullable: false),
                    MoneyInside_Yuan2 = table.Column<int>(type: "int", nullable: false),
                    MoneyInside_Yuan5 = table.Column<int>(type: "int", nullable: false),
                    MoneyInside_Yuan10 = table.Column<int>(type: "int", nullable: false),
                    MoneyInside_Yuan20 = table.Column<int>(type: "int", nullable: false),
                    MoneyInside_Yuan50 = table.Column<int>(type: "int", nullable: false),
                    MoneyInside_Yuan100 = table.Column<int>(type: "int", nullable: false),
                    MoneyInside_Amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    AmountInTransaction = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    SlotsCount = table.Column<int>(type: "int", nullable: false),
                    SnackCount = table.Column<int>(type: "int", nullable: false),
                    SnackQuantity = table.Column<int>(type: "int", nullable: false),
                    SnackAmount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
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
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
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
                name: "MachineSnackStats",
                columns: table => new
                {
                    MachineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SnackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalQuantity = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineSnackStats", x => new { x.MachineId, x.SnackId });
                    table.ForeignKey(
                        name: "FK_MachineSnackStats_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MachineSnackStats_Snacks_SnackId",
                        column: x => x.SnackId,
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
                name: "IX_Machines_IsDeleted",
                table: "Machines",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_MachineSlots_SnackPile_SnackId",
                table: "MachineSlots",
                column: "SnackPile_SnackId");

            migrationBuilder.CreateIndex(
                name: "IX_MachineSnackStats_SnackId",
                table: "MachineSnackStats",
                column: "SnackId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_MachineId",
                table: "Purchases",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_SnackId",
                table: "Purchases",
                column: "SnackId");

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
                name: "MachineSnackStats");

            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropTable(
                name: "Machines");

            migrationBuilder.DropTable(
                name: "Snacks");
        }
    }
}
