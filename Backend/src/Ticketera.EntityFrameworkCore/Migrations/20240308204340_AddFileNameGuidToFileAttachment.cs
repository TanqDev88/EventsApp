using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ticketera.Migrations
{
    /// <inheritdoc />
    public partial class AddFileNameGuidToFileAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileNameGuid",
                table: "FileAttachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"UPDATE [dbo].[FileAttachments]
            SET [FileNameGuid] = CAST(NEWID() AS NVARCHAR(36)) + '.' + RIGHT([FileName], CHARINDEX('.', REVERSE([FileName])) - 1)
            WHERE IsDeleted = 0;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileNameGuid",
                table: "FileAttachments");
        }
    }
}
