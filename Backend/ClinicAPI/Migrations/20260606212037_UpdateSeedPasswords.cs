using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$4BEOTWkbHvboS6Nc9uM2P.rpQjvxmH5QuMcUzOp7Gl/EBys5hA2hK");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$Ot4Z9wnGRAz4tCBkzPWbOeGUDFoFJCy1Us3t/8.DqLIy9MiN6s4Aq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$Ot4Z9wnGRAz4tCBkzPWbOeGUDFoFJCy1Us3t/8.DqLIy9MiN6s4Aq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$Ot4Z9wnGRAz4tCBkzPWbOeGUDFoFJCy1Us3t/8.DqLIy9MiN6s4Aq");
        }
    }
}
