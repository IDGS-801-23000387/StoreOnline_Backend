using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreOnline_Backend.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionModelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Productos_Categoria",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_Nombre",
                table: "Productos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Productos_Categoria",
                table: "Productos",
                column: "Categoria");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_Nombre",
                table: "Productos",
                column: "Nombre");
        }
    }
}
