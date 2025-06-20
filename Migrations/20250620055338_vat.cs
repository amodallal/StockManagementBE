using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockManagement.Migrations
{
    /// <inheritdoc />
    public partial class vat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "vat",
                table: "orders",
                type: "decimal(10,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "vat",
                table: "orders");
        }
    }
}
