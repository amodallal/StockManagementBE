using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddItemCapacityManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemCapacity",
                columns: table => new
                {
                    CapacitiesCapacityID = table.Column<int>(type: "int", nullable: false),
                    ItemsItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCapacity", x => new { x.CapacitiesCapacityID, x.ItemsItemId });
                    table.ForeignKey(
                        name: "FK_ItemCapacity_Capacities_CapacitiesCapacityID",
                        column: x => x.CapacitiesCapacityID,
                        principalTable: "Capacities",
                        principalColumn: "CapacityID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemCapacity_items_ItemsItemId",
                        column: x => x.ItemsItemId,
                        principalTable: "items",
                        principalColumn: "item_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemCapacity_ItemsItemId",
                table: "ItemCapacity",
                column: "ItemsItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemCapacity");
        }
    }
}
