using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vending.Domain.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddVersionColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Snacks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Machines",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Snacks");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Machines");
        }
    }
}
