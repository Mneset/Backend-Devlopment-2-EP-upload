using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedNavigationPropForDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Clinic_ClinicId1",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ClinicId1",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ClinicId1",
                table: "Appointments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClinicId1",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ClinicId1",
                table: "Appointments",
                column: "ClinicId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Clinic_ClinicId1",
                table: "Appointments",
                column: "ClinicId1",
                principalTable: "Clinic",
                principalColumn: "Id");
        }
    }
}
