using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicAPI.Migrations
{
    /// <inheritdoc />
    public partial class EnrichPatientSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DriverLicenseNumber", "MedicalInsuranceNumber", "Religion", "SocialSecurityNumber", "TaxNumber" },
                values: new object[] { "DL10001", "MI10001", 7, "15059012345", "TX10001" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DriverLicenseNumber", "MedicalInsuranceNumber", "Religion", "SocialSecurityNumber", "TaxNumber" },
                values: new object[] { "DL10002", "MI10002", 7, "22038512345", "TX10002" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DriverLicenseNumber", "MedicalInsuranceNumber", "Religion", "SocialSecurityNumber", "TaxNumber" },
                values: new object[] { null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DriverLicenseNumber", "MedicalInsuranceNumber", "Religion", "SocialSecurityNumber", "TaxNumber" },
                values: new object[] { null, null, null, null, null });
        }
    }
}
