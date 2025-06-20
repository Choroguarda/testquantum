using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestQuantumDocs.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "page",
                table: "DocumentPageIndexes",
                newName: "Page");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "DocumentPageIndexes",
                newName: "Name");

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Documents",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "Page",
                table: "DocumentPageIndexes",
                newName: "page");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "DocumentPageIndexes",
                newName: "name");
        }
    }
}
