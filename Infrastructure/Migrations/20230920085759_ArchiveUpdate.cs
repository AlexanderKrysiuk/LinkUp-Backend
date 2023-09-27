using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LinkUpBackend.Migrations
{
    /// <inheritdoc />
    public partial class ArchiveUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeetingsOrganizators_Meeting_MeetingId",
                table: "MeetingsOrganizators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Meeting",
                table: "Meeting");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "28d64ac4-e685-4cea-93d1-b244ba36929e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2ed05b49-11ab-414f-ab0d-32300f368346");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c83cb1a6-d79e-4f5c-beb7-18a74d447638");

            migrationBuilder.RenameTable(
                name: "Meeting",
                newName: "Meetings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Meetings",
                table: "Meetings",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Archive",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Archive", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "095d7b3b-826c-40dd-b64d-4e5c0916dd76", null, "Admin", "ADMIN" },
                    { "0aecf550-53ad-4363-8729-056ea7a2a407", null, "Client", "CLIENT" },
                    { "1285b52d-615e-471c-9418-f340cc7832e4", null, "Contractor", "CONTRACTOR" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_MeetingsOrganizators_Meetings_MeetingId",
                table: "MeetingsOrganizators",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeetingsOrganizators_Meetings_MeetingId",
                table: "MeetingsOrganizators");

            migrationBuilder.DropTable(
                name: "Archive");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Meetings",
                table: "Meetings");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "095d7b3b-826c-40dd-b64d-4e5c0916dd76");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0aecf550-53ad-4363-8729-056ea7a2a407");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1285b52d-615e-471c-9418-f340cc7832e4");

            migrationBuilder.RenameTable(
                name: "Meetings",
                newName: "Meeting");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Meeting",
                table: "Meeting",
                column: "Id");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "28d64ac4-e685-4cea-93d1-b244ba36929e", null, "Contractor", "CONTRACTOR" },
                    { "2ed05b49-11ab-414f-ab0d-32300f368346", null, "Admin", "ADMIN" },
                    { "c83cb1a6-d79e-4f5c-beb7-18a74d447638", null, "Client", "CLIENT" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_MeetingsOrganizators_Meeting_MeetingId",
                table: "MeetingsOrganizators",
                column: "MeetingId",
                principalTable: "Meeting",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
