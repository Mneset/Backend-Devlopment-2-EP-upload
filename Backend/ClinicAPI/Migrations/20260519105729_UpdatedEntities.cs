using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClinicAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Patients");

            migrationBuilder.AlterColumn<int>(
                name: "Religion",
                table: "Patients",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Gender",
                table: "Patients",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Doctors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Clinic",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AppointmentCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "General Checkup" },
                    { 2, "Follow-up Consultation" },
                    { 3, "Specialist Referral" },
                    { 4, "Vaccination" },
                    { 5, "Lab Test" }
                });

            migrationBuilder.UpdateData(
                table: "Clinic",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "IsActive", "Name" },
                values: new object[] { true, "Oslo University Hospital" });

            migrationBuilder.UpdateData(
                table: "Clinic",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "IsActive", "Name" },
                values: new object[] { true, "Ullevål Medical Center" });

            migrationBuilder.UpdateData(
                table: "Clinic",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "IsActive", "Name" },
                values: new object[] { true, "Rikshospitalet Clinic" });

            migrationBuilder.InsertData(
                table: "Clinic",
                columns: new[] { "Id", "IsActive", "Name" },
                values: new object[,]
                {
                    { 4, true, "Drammen Health Clinic" },
                    { 5, true, "Ahus Outpatient Clinic" }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "DateOfBirth", "DriverLicenseNumber", "FirstName", "Gender", "IsActive", "IsRegistered", "LastName", "MedicalInsuranceNumber", "Religion", "SocialSecurityNumber", "TaxNumber" },
                values: new object[,]
                {
                    { 1, new DateTime(1990, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Jonas", 0, true, true, "Nilsen", null, null, null, null },
                    { 2, new DateTime(1985, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Marie", 1, true, true, "Andersen", null, null, null, null },
                    { 3, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Guest", 0, true, false, "User", null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Specialities",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Cardiology" },
                    { 2, "Dermatology" },
                    { 3, "Neurology" },
                    { 4, "Orthopedics" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "PatientId", "Role" },
                values: new object[] { 1, "admin@clinic.com", "$2a$11$Ot4Z9wnGRAz4tCBkzPWbOeGUDFoFJCy1Us3t/8.DqLIy9MiN6s4Aq", null, 0 });

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "Id", "ClinicId", "FirstName", "Gender", "IsActive", "LastName", "SpecialityId" },
                values: new object[,]
                {
                    { 1, 1, "Erik", 0, true, "Hansen", 1 },
                    { 2, 2, "Anna", 1, true, "Olsen", 2 },
                    { 3, 3, "Lars", 0, true, "Berg", 3 },
                    { 4, 4, "Sofia", 1, true, "Dahl", 4 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "PatientId", "Role" },
                values: new object[,]
                {
                    { 2, "jonas@email.com", "$2a$11$Ot4Z9wnGRAz4tCBkzPWbOeGUDFoFJCy1Us3t/8.DqLIy9MiN6s4Aq", 1, 1 },
                    { 3, "marie@email.com", "$2a$11$Ot4Z9wnGRAz4tCBkzPWbOeGUDFoFJCy1Us3t/8.DqLIy9MiN6s4Aq", 2, 1 },
                    { 4, "guest@email.com", null, 3, 2 }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "AppointmentCategoryId", "AppointmentDate", "ClinicId", "DoctorId", "DurationInMin", "PatientId", "Status" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 6, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 30, 1, 0 },
                    { 2, 2, new DateTime(2026, 6, 11, 10, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, 45, 2, 0 },
                    { 3, 3, new DateTime(2026, 5, 1, 14, 0, 0, 0, DateTimeKind.Unspecified), 3, 3, 60, 1, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PatientId",
                table: "Users",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DeleteData(
                table: "AppointmentCategories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AppointmentCategories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Clinic",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AppointmentCategories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AppointmentCategories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AppointmentCategories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Clinic",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Specialities",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Specialities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Specialities",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Specialities",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Clinic");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Appointments");

            migrationBuilder.AlterColumn<string>(
                name: "Religion",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Clinic",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "First Clinic");

            migrationBuilder.UpdateData(
                table: "Clinic",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Second Clinic");

            migrationBuilder.UpdateData(
                table: "Clinic",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Third Clinic");
        }
    }
}
