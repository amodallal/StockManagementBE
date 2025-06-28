using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecIdToItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpecsId",
                table: "items",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_items_SpecsId",
                table: "items",
                column: "SpecsId");

            migrationBuilder.AddForeignKey(
                name: "FK_items_Specs_SpecsId",
                table: "items",
                column: "SpecsId",
                principalTable: "Specs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_Specs_SpecsId",
                table: "items");

            migrationBuilder.DropIndex(
                name: "IX_items_SpecsId",
                table: "items");

            migrationBuilder.DropColumn(
                name: "SpecsId",
                table: "items");
        }
    }
}
