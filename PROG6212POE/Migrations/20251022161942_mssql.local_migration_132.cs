using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG6212POE.Migrations
{
    /// <inheritdoc />
    public partial class mssqllocal_migration_132 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "HoursWorked",
                table: "ClaimModel",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "HoursWorked",
                table: "ClaimModel",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
