using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LinkUpBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitializeMeeting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "Meetings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaxParticipant = table.Column<int>(type: "integer", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meetings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeetingsOrganizators",
                columns: table => new
                {
                    OrganizatorId = table.Column<string>(type: "text", nullable: false),
                    MeetingId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingsOrganizators", x => new { x.OrganizatorId, x.MeetingId });
                    table.ForeignKey(
                        name: "FK_MeetingsOrganizators_AspNetUsers_OrganizatorId",
                        column: x => x.OrganizatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeetingsOrganizators_Meetings_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "Meetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3a259dae-321a-4ca3-93f8-8def38c0bfcf", null, "Client", "CLIENT" },
                    { "6f07bef7-bee9-4526-ac9e-bc5c027d6c6b", null, "Admin", "ADMIN" },
                    { "97ea2e03-25a8-467f-bb31-20165f8a4eff", null, "Contractor", "CONTRACTOR" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeetingsOrganizators_MeetingId",
                table: "MeetingsOrganizators",
                column: "MeetingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeetingsOrganizators");

            migrationBuilder.DropTable(
                name: "Meetings");

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
    }
}
