using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ticketera.Migrations
{
    /// <inheritdoc />
    public partial class ChangePropertiesToPurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExtraObject",
                table: "Purchases",
                newName: "ExtraProperties");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExtraProperties",
                table: "Purchases",
                newName: "ExtraObject");
        }
    }
}
