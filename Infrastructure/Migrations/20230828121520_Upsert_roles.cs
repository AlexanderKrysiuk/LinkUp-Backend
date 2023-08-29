using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LinkUpBackend.Migrations
{
    /// <inheritdoc />
    public partial class Upsert_roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6485aa08-1e2b-43b0-a721-48ab6a655df6", null, "Client", "CLIENT" },
                    { "7d6c19c6-0d1c-48c8-8962-3344c2e2f528", null, "Admin", "ADMIN" },
                    { "f9502ec8-82ee-45fa-a185-8d2763bbd2c9", null, "Contractor", "CONTRACTOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6485aa08-1e2b-43b0-a721-48ab6a655df6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7d6c19c6-0d1c-48c8-8962-3344c2e2f528");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f9502ec8-82ee-45fa-a185-8d2763bbd2c9");
        }
    }
}
