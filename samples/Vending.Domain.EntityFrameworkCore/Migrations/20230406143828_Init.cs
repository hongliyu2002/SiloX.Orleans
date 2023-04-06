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
                name: "SnackMachines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    MoneyInside_Yuan1 = table.Column<int>(type: "int", nullable: false),
                    MoneyInside_Yuan2 = table.Column<int>(type: "int", nullable: false),
                    MoneyInside_Yuan5 = table.Column<int>(type: "int", nullable: false),
                    MoneyInside_Yuan10 = table.Column<int>(type: "int", nullable: false),
                    MoneyInside_Yuan20 = table.Column<int>(type: "int", nullable: false),
                    MoneyInside_Yuan50 = table.Column<int>(type: "int", nullable: false),
                    MoneyInside_Yuan100 = table.Column<int>(type: "int", nullable: false),
                    AmountInTransaction = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SnackMachines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Snacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsCreated = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PictureUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snacks", x => x.Id);
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
                    BoughtBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Purchases_SnackMachines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "SnackMachines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Purchases_Snacks_SnackId",
                        column: x => x.SnackId,
                        principalTable: "Snacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Slots",
                columns: table => new
                {
                    MachineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    SnackPile_SnackId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SnackPile_Quantity = table.Column<int>(type: "int", nullable: true),
                    SnackPile_Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slots", x => new { x.MachineId, x.Position });
                    table.ForeignKey(
                        name: "FK_Slots_SnackMachines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "SnackMachines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Slots_Snacks_SnackPile_SnackId",
                        column: x => x.SnackPile_SnackId,
                        principalTable: "Snacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_MachineId",
                table: "Purchases",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_SnackId",
                table: "Purchases",
                column: "SnackId");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_SnackPile_SnackId",
                table: "Slots",
                column: "SnackPile_SnackId");

            migrationBuilder.CreateIndex(
                name: "IX_Snacks_IsDeleted_Name",
                table: "Snacks",
                columns: new[] { "IsDeleted", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropTable(
                name: "Slots");

            migrationBuilder.DropTable(
                name: "SnackMachines");

            migrationBuilder.DropTable(
                name: "Snacks");
        }
    }
}
