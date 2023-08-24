using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkUp.Migrations
{
    /// <inheritdoc />
    public partial class Contractors_unique_email : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Contractors_Email",
                table: "Contractors",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contractors_Email",
                table: "Contractors");
        }
    }
}
