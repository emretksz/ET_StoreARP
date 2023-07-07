using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class dende : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductAges_ProductAgesId",
                table: "Products");

            migrationBuilder.AlterColumn<long>(
                name: "ProductAgesId",
                table: "Products",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductAges_ProductAgesId",
                table: "Products",
                column: "ProductAgesId",
                principalTable: "ProductAges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductAges_ProductAgesId",
                table: "Products");

            migrationBuilder.AlterColumn<long>(
                name: "ProductAgesId",
                table: "Products",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductAges_ProductAgesId",
                table: "Products",
                column: "ProductAgesId",
                principalTable: "ProductAges",
                principalColumn: "Id");
        }
    }
}
