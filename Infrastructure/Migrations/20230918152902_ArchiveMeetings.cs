using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LinkUpBackend.Migrations
{
    /// <inheritdoc />
    public partial class ArchiveMeetings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeetingsOrganizators_Meetings_MeetingId",
                table: "MeetingsOrganizators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Meetings",
                table: "Meetings");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5521aae1-42ba-4627-989f-d3f97c39f9b1", null, "Contractor", "CONTRACTOR" },
                    { "5b39b385-7a0a-44bd-a7b5-1c7e85a0a76c", null, "Client", "CLIENT" },
                    { "de07ed46-4bc0-46eb-98d9-63b5250e0725", null, "Admin", "ADMIN" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_MeetingsOrganizators_Meetings_MeetingId",
                table: "MeetingsOrganizators",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
