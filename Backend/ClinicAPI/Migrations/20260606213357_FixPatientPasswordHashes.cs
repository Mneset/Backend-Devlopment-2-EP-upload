using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixPatientPasswordHashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK");
        }
    }
}
