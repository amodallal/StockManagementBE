using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockManagement.Migrations
{
    /// <inheritdoc />
    public partial class salesmanstockStatusRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_salesman_stock_status_id",
                table: "salesman_stock",
                column: "status_id");

            migrationBuilder.AddForeignKey(
                name: "fk_salesman_stock_status",
                table: "salesman_stock",
                column: "status_id",
                principalTable: "status",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_salesman_stock_status",
                table: "salesman_stock");

            migrationBuilder.DropIndex(
                name: "IX_salesman_stock_status_id",
                table: "salesman_stock");
        }
    }
}
