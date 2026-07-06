using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bibliotheca.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBookFilterIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Books_ConditionEnum",
                table: "Books",
                column: "ConditionEnum");

            migrationBuilder.CreateIndex(
                name: "IX_Books_CreatedAt",
                table: "Books",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Books_EstimatedValue",
                table: "Books",
                column: "EstimatedValue");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Language",
                table: "Books",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Pages",
                table: "Books",
                column: "Pages");

            migrationBuilder.CreateIndex(
                name: "IX_Books_PublicationYear",
                table: "Books",
                column: "PublicationYear");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Books_ConditionEnum",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_CreatedAt",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_EstimatedValue",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_Language",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_Pages",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_PublicationYear",
                table: "Books");
        }
    }
}
