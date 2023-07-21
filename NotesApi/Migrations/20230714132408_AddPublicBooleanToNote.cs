using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotesApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicBooleanToNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Public",
                table: "Notes",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Public",
                table: "Notes");
        }
    }
}
