using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ticketera.Migrations
{
    /// <inheritdoc />
    public partial class AddBackgroundJobIdToEventDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackgroundJobId",
                table: "EventDates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundJobId",
                table: "EventDates");
        }
    }
}
