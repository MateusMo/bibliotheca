using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bibliotheca.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBookViewCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ViewCount",
                table: "Books",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Books_ViewCount",
                table: "Books",
                column: "ViewCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Books_ViewCount",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Books");
        }
    }
}
