using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlgebraWebShop2026.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedPropertyNameInApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ZIP",
                table: "AspNetUsers",
                newName: "PB");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "AspNetUsers",
                newName: "Grad");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "AspNetUsers",
                newName: "Drzava");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PB",
                table: "AspNetUsers",
                newName: "ZIP");

            migrationBuilder.RenameColumn(
                name: "Grad",
                table: "AspNetUsers",
                newName: "Country");

            migrationBuilder.RenameColumn(
                name: "Drzava",
                table: "AspNetUsers",
                newName: "City");
        }
    }
}
