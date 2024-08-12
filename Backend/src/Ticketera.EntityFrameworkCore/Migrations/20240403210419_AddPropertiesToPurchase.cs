using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ticketera.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertiesToPurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "Purchases");
        }
    }
}
