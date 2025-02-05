using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockManagement.Migrations
{
    /// <inheritdoc />
    public partial class SSID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_item_details_id",
                table: "ordered_items");

            migrationBuilder.DropPrimaryKey(
                name: "PK__salesman__7D6D00AEBDF35074",
                table: "salesman_stock");

            migrationBuilder.AddColumn<int>(
                name: "SalesmanStockId",
                table: "salesman_stock",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK__salesman__7D6D00AEBDF35074",
                table: "salesman_stock",
                column: "SalesmanStockId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropPrimaryKey(
                name: "PK__salesman__7D6D00AEBDF35074",
                table: "salesman_stock");

            migrationBuilder.DropColumn(
                name: "SalesmanStockId",
                table: "salesman_stock");

            migrationBuilder.AddPrimaryKey(
                name: "PK__salesman__7D6D00AEBDF35074",
                table: "salesman_stock",
                column: "item_details_id");

            migrationBuilder.AddForeignKey(
                name: "fk_item_details_id",
                table: "ordered_items",
                column: "item_details_id",
                principalTable: "salesman_stock",
                principalColumn: "item_details_id");
        }
    }
}
