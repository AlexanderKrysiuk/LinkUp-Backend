using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LinkUpBackend.Migrations
{
    /// <inheritdoc />
    public partial class MeetingsParticipants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "MeetingsParticipants",
                columns: table => new
                {
                    ParticipantId = table.Column<string>(type: "text", nullable: false),
                    MeetingId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingsParticipants", x => new { x.ParticipantId, x.MeetingId });
                    table.ForeignKey(
                        name: "FK_MeetingsParticipants_AspNetUsers_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeetingsParticipants_Meetings_MeetingId",
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
                    { "2aa44a11-7b8b-45e8-9db0-9f7260169d17", null, "Client", "CLIENT" },
                    { "56f82fdd-9d03-443d-b781-ed3e88fc2e00", null, "Contractor", "CONTRACTOR" },
                    { "ac35f991-85ed-4143-8076-a84ef06a10fe", null, "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeetingsParticipants_MeetingId",
                table: "MeetingsParticipants",
                column: "MeetingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeetingsParticipants");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2aa44a11-7b8b-45e8-9db0-9f7260169d17");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "56f82fdd-9d03-443d-b781-ed3e88fc2e00");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ac35f991-85ed-4143-8076-a84ef06a10fe");

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
    }
}
