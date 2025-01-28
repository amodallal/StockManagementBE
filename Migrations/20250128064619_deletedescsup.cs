using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockManagement.Migrations
{
    /// <inheritdoc />
    public partial class deletedescsup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description_id",
                table: "salesman_stock");

            migrationBuilder.DropColumn(
                name: "supplier_id",
                table: "salesman_stock");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "description_id",
                table: "salesman_stock",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "supplier_id",
                table: "salesman_stock",
                type: "int",
                nullable: true);
        }
    }
}
