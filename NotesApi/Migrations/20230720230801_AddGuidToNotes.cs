using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotesApi.Migrations
{
    /// <inheritdoc />
    public partial class AddGuidToNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Guid",
                table: "Notes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Notes");
        }
    }
}
