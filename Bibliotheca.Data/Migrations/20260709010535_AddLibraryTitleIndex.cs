using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bibliotheca.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLibraryTitleIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Libraries_Title",
                table: "Libraries",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Libraries_Title",
                table: "Libraries");
        }
    }
}
