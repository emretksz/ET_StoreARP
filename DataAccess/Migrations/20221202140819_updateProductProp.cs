using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class updateProductProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProductAgesId",
                table: "Products",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductAges",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAges", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductAgesId",
                table: "Products",
                column: "ProductAgesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductAges_ProductAgesId",
                table: "Products",
                column: "ProductAgesId",
                principalTable: "ProductAges",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductAges_ProductAgesId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ProductAges");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductAgesId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductAgesId",
                table: "Products");
        }
    }
}
