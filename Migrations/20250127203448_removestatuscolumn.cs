using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockManagement.Migrations
{
    /// <inheritdoc />
    public partial class removestatuscolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "delivery_status",
                table: "deliveries",
                newName: "status_id");

            migrationBuilder.AlterColumn<int>(
                name: "status_id",
                table: "deliveries",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValue: "Pending");


            migrationBuilder.CreateIndex(
                name: "IX_deliveries_status_id",
                table: "deliveries",
                column: "status_id");

            migrationBuilder.AddForeignKey(
                name: "FK_deliveries_status_status_id",
                table: "deliveries",
                column: "status_id",
                principalTable: "status",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_deliveries_status_status_id",
                table: "deliveries");

            migrationBuilder.DropIndex(
                name: "IX_deliveries_status_id",
                table: "deliveries");

            migrationBuilder.DropColumn(
                name: "Status_Id",
                table: "deliveries");

            migrationBuilder.RenameColumn(
                name: "status_id",
                table: "deliveries",
                newName: "delivery_status");

            migrationBuilder.AlterColumn<string>(
                name: "delivery_status",
                table: "deliveries",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true,
                defaultValue: "Pending",
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
