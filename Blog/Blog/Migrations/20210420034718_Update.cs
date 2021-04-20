using Microsoft.EntityFrameworkCore.Migrations;

namespace Blog.Migrations
{
    public partial class Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_Product_PostID",
                table: "ProductCategories");

            migrationBuilder.DropIndex(
                name: "IX_ProductCategories_PostID",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "PostID",
                table: "ProductCategories");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_Product_ProductID",
                table: "ProductCategories",
                column: "ProductID",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_Product_ProductID",
                table: "ProductCategories");

            migrationBuilder.AddColumn<int>(
                name: "PostID",
                table: "ProductCategories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_PostID",
                table: "ProductCategories",
                column: "PostID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_Product_PostID",
                table: "ProductCategories",
                column: "PostID",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
