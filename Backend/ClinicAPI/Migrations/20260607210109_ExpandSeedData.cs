using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClinicAPI.Migrations
{
    /// <inheritdoc />
    public partial class ExpandSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "AppointmentCategoryId", "AppointmentDate", "ClinicId", "DoctorId", "DurationInMin", "PatientId", "Status" },
                values: new object[,]
                {
                    { 14, 1, new DateTime(2026, 2, 20, 10, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 30, 1, 1 },
                    { 19, 5, new DateTime(2026, 4, 18, 16, 0, 0, 0, DateTimeKind.Unspecified), 3, 3, 30, 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "Id", "ClinicId", "FirstName", "Gender", "IsActive", "LastName", "SpecialityId" },
                values: new object[,]
                {
                    { 5, 2, "Olav", 0, true, "Lund", 1 },
                    { 6, 5, "Ingrid", 1, true, "Solberg", 1 },
                    { 7, 4, "Maria", 1, true, "Strand", 2 },
                    { 8, 1, "Henrik", 0, true, "Vik", 2 },
                    { 9, 5, "Astrid", 1, true, "Bakke", 3 },
                    { 10, 2, "Tor", 0, true, "Iversen", 4 },
                    { 11, 3, "Linnea", 1, true, "Holm", 4 },
                    { 12, 1, "Magnus", 0, true, "Eide", 3 }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "DateOfBirth", "DriverLicenseNumber", "FirstName", "Gender", "IsActive", "IsRegistered", "LastName", "MedicalInsuranceNumber", "Religion", "SocialSecurityNumber", "TaxNumber" },
                values: new object[,]
                {
                    { 4, new DateTime(1988, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "DL10004", "Henrik", 0, true, true, "Aas", "MI10004", 0, "12048812345", "TX10004" },
                    { 5, new DateTime(1995, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "DL10005", "Kari", 1, true, true, "Solheim", "MI10005", 6, "03119512345", "TX10005" },
                    { 6, new DateTime(1972, 7, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "DL10006", "Ola", 0, true, true, "Lien", "MI10006", 7, "28077212345", "TX10006" },
                    { 7, new DateTime(1992, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "DL10007", "Ingvild", 1, true, true, "Moen", "MI10007", 5, "19029212345", "TX10007" },
                    { 8, new DateTime(1980, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "DL10008", "Sven", 0, true, true, "Berge", "MI10008", 0, "05098012345", "TX10008" },
                    { 9, new DateTime(1998, 12, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "DL10009", "Astrid", 1, true, true, "Kvam", "MI10009", 6, "30129812345", "TX10009" },
                    { 10, new DateTime(1965, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Per", 0, true, false, "Walk-in", null, null, null, null },
                    { 11, new DateTime(1978, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Lisa", 1, true, false, "Visitor", null, null, null, null },
                    { 12, new DateTime(2002, 6, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Thomas", 0, true, false, "Drop-in", null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "AppointmentCategoryId", "AppointmentDate", "ClinicId", "DoctorId", "DurationInMin", "PatientId", "Status" },
                values: new object[,]
                {
                    { 4, 1, new DateTime(2026, 6, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), 2, 5, 30, 1, 0 },
                    { 5, 2, new DateTime(2026, 6, 20, 11, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 45, 4, 0 },
                    { 6, 1, new DateTime(2026, 6, 25, 14, 0, 0, 0, DateTimeKind.Unspecified), 1, 8, 30, 5, 0 },
                    { 7, 3, new DateTime(2026, 7, 2, 9, 0, 0, 0, DateTimeKind.Unspecified), 3, 11, 60, 6, 0 },
                    { 8, 1, new DateTime(2026, 7, 10, 13, 30, 0, 0, DateTimeKind.Unspecified), 5, 9, 30, 7, 0 },
                    { 9, 4, new DateTime(2026, 7, 15, 16, 0, 0, 0, DateTimeKind.Unspecified), 4, 4, 15, 8, 0 },
                    { 10, 5, new DateTime(2026, 7, 20, 10, 30, 0, 0, DateTimeKind.Unspecified), 2, 2, 30, 9, 0 },
                    { 11, 2, new DateTime(2026, 5, 15, 9, 0, 0, 0, DateTimeKind.Unspecified), 1, 12, 45, 4, 0 },
                    { 12, 1, new DateTime(2026, 4, 10, 11, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 30, 2, 1 },
                    { 13, 5, new DateTime(2026, 3, 15, 14, 0, 0, 0, DateTimeKind.Unspecified), 5, 6, 45, 5, 1 },
                    { 15, 3, new DateTime(2026, 1, 15, 13, 0, 0, 0, DateTimeKind.Unspecified), 4, 4, 60, 6, 1 },
                    { 16, 4, new DateTime(2026, 4, 25, 15, 30, 0, 0, DateTimeKind.Unspecified), 4, 7, 15, 8, 1 },
                    { 17, 1, new DateTime(2026, 5, 5, 10, 0, 0, 0, DateTimeKind.Unspecified), 2, 5, 30, 7, 2 },
                    { 18, 2, new DateTime(2026, 6, 28, 14, 30, 0, 0, DateTimeKind.Unspecified), 3, 11, 45, 9, 2 },
                    { 20, 4, new DateTime(2026, 6, 12, 11, 30, 0, 0, DateTimeKind.Unspecified), 4, 4, 15, 10, 0 },
                    { 21, 1, new DateTime(2026, 4, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, 30, 11, 1 },
                    { 22, 1, new DateTime(2026, 6, 30, 11, 0, 0, 0, DateTimeKind.Unspecified), 5, 6, 30, 12, 0 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "PatientId", "Role" },
                values: new object[,]
                {
                    { 5, "henrik@email.com", "$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK", 4, 1 },
                    { 6, "kari@email.com", "$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK", 5, 1 },
                    { 7, "ola@email.com", "$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK", 6, 1 },
                    { 8, "ingvild@email.com", "$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK", 7, 1 },
                    { 9, "sven@email.com", "$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK", 8, 1 },
                    { 10, "astrid@email.com", "$2a$11$kaXznudJD90DTmdfrDexeOhjwZkhE06T0EAPLVooywIXndkYY90tK", 9, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 12);
        }
    }
}
