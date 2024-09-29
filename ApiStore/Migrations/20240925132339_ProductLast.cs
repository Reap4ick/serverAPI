using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiStore.Migrations
{
    /// <inheritdoc />
    public partial class ProductLast : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblProductImages_tblProducts_ProductId",
                table: "tblProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_tblProducts_tblCategories_CategoryId",
                table: "tblProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblProducts",
                table: "tblProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblProductImages",
                table: "tblProductImages");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "tblProducts");

            migrationBuilder.RenameTable(
                name: "tblProducts",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "tblProductImages",
                newName: "ProductImages");

            migrationBuilder.RenameIndex(
                name: "IX_tblProducts_CategoryId",
                table: "Products",
                newName: "IX_Products_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_tblProductImages_ProductId",
                table: "ProductImages",
                newName: "IX_ProductImages_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductImages",
                table: "ProductImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Products_ProductId",
                table: "ProductImages",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_tblCategories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "tblCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Products_ProductId",
                table: "ProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_tblCategories_CategoryId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductImages",
                table: "ProductImages");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "tblProducts");

            migrationBuilder.RenameTable(
                name: "ProductImages",
                newName: "tblProductImages");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CategoryId",
                table: "tblProducts",
                newName: "IX_tblProducts_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductImages_ProductId",
                table: "tblProductImages",
                newName: "IX_tblProductImages_ProductId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "tblProducts",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblProducts",
                table: "tblProducts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblProductImages",
                table: "tblProductImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblProductImages_tblProducts_ProductId",
                table: "tblProductImages",
                column: "ProductId",
                principalTable: "tblProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tblProducts_tblCategories_CategoryId",
                table: "tblProducts",
                column: "CategoryId",
                principalTable: "tblCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
