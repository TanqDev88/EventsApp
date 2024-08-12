using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ticketera.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyToTicketSectorModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "TicketSectors",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "TicketSectors");
        }
    }
}
