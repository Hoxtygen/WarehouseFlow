using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReservedQuantityFromReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReservedQuantity",
                table: "reservations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservedQuantity",
                table: "reservations");
        }
    }
}
