using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveEmployeeCreatorAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_CreatedByUserId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CreatedByUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "employees",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "employees",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_employees_CreatedByUserId",
                table: "employees",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_employees_AspNetUsers_CreatedByUserId",
                table: "employees",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employees_AspNetUsers_CreatedByUserId",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "IX_employees_CreatedByUserId",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "employees");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "employees",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CreatedByUserId",
                table: "AspNetUsers",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_CreatedByUserId",
                table: "AspNetUsers",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
