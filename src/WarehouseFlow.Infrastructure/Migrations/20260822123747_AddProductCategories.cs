using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "products");

            migrationBuilder.AlterColumn<string>(
                name: "Brand",
                table: "products",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductCategoryId",
                table: "products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "product_categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    CategoryName = table.Column<string>(type: "text", nullable: false),
                    CategorySlug = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_categories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_products_ProductCategoryId",
                table: "products",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_product_categories_CategoryName",
                table: "product_categories",
                column: "CategoryName");

            migrationBuilder.CreateIndex(
                name: "IX_product_categories_CategorySlug",
                table: "product_categories",
                column: "CategorySlug");

            migrationBuilder.AddForeignKey(
                name: "FK_products_product_categories_ProductCategoryId",
                table: "products",
                column: "ProductCategoryId",
                principalTable: "product_categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_product_categories_ProductCategoryId",
                table: "products");

            migrationBuilder.DropTable(
                name: "product_categories");

            migrationBuilder.DropIndex(
                name: "IX_products_ProductCategoryId",
                table: "products");

            migrationBuilder.DropColumn(
                name: "ProductCategoryId",
                table: "products");

            migrationBuilder.AlterColumn<string>(
                name: "Brand",
                table: "products",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "products",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
