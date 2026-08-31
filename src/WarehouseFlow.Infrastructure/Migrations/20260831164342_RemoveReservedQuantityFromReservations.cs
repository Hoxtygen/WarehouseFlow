using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReservedQuantityFromReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservedQuantity",
                table: "reservations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReservedQuantity",
                table: "reservations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
