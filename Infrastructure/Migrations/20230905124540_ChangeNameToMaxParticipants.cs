using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LinkUpBackend.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNameToMaxParticipants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3a259dae-321a-4ca3-93f8-8def38c0bfcf");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6f07bef7-bee9-4526-ac9e-bc5c027d6c6b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97ea2e03-25a8-467f-bb31-20165f8a4eff");

            migrationBuilder.RenameColumn(
                name: "MaxParticipant",
                table: "Meetings",
                newName: "MaxParticipants");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Meetings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5521aae1-42ba-4627-989f-d3f97c39f9b1", null, "Contractor", "CONTRACTOR" },
                    { "5b39b385-7a0a-44bd-a7b5-1c7e85a0a76c", null, "Client", "CLIENT" },
                    { "de07ed46-4bc0-46eb-98d9-63b5250e0725", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5521aae1-42ba-4627-989f-d3f97c39f9b1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5b39b385-7a0a-44bd-a7b5-1c7e85a0a76c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "de07ed46-4bc0-46eb-98d9-63b5250e0725");

            migrationBuilder.RenameColumn(
                name: "MaxParticipants",
                table: "Meetings",
                newName: "MaxParticipant");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Meetings",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3a259dae-321a-4ca3-93f8-8def38c0bfcf", null, "Client", "CLIENT" },
                    { "6f07bef7-bee9-4526-ac9e-bc5c027d6c6b", null, "Admin", "ADMIN" },
                    { "97ea2e03-25a8-467f-bb31-20165f8a4eff", null, "Contractor", "CONTRACTOR" }
                });
        }
    }
}
