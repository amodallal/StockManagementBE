using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockManagement.Migrations
{
    /// <inheritdoc />
    public partial class updatecolumnname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsIemiId",
                table: "items");

            migrationBuilder.AddColumn<bool>(
                name: "IsImeiId",
                table: "items",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsImeiId",
                table: "items");

            migrationBuilder.AddColumn<bool>(
                name: "IsIemiId",
                table: "items",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
