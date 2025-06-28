using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockManagement.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCategoryToOneToManySpecs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Specs_CategoryId",
                table: "Specs");

            migrationBuilder.CreateIndex(
                name: "IX_Specs_CategoryId",
                table: "Specs",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Specs_CategoryId",
                table: "Specs");

            migrationBuilder.CreateIndex(
                name: "IX_Specs_CategoryId",
                table: "Specs",
                column: "CategoryId",
                unique: true);
        }
    }
}
