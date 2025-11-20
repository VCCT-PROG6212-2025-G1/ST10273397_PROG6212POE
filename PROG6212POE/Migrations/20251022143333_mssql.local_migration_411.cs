using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG6212POE.Migrations
{
    /// <inheritdoc />
    public partial class mssqllocal_migration_411 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LecturerName",
                table: "ClaimModel",
                newName: "AdditionalNotes");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "ClaimModel",
                newName: "HoursWorked");

            migrationBuilder.AddColumn<int>(
                name: "HourlyRate",
                table: "ClaimModel",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HourlyRate",
                table: "ClaimModel");

            migrationBuilder.RenameColumn(
                name: "HoursWorked",
                table: "ClaimModel",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "AdditionalNotes",
                table: "ClaimModel",
                newName: "LecturerName");
        }
    }
}
