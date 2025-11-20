using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG6212POE.Migrations
{
    /// <inheritdoc />
    public partial class mssqllocal_migration_149 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SuppDoc",
                table: "ClaimModel",
                newName: "SuppDocPath");

            migrationBuilder.AddColumn<string>(
                name: "SuppDocName",
                table: "ClaimModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuppDocName",
                table: "ClaimModel");

            migrationBuilder.RenameColumn(
                name: "SuppDocPath",
                table: "ClaimModel",
                newName: "SuppDoc");
        }
    }
}
