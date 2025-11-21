using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG6212POE.Migrations
{
    /// <inheritdoc />
    public partial class AddedSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserModel",
                columns: new[] { "UserId", "Email", "FirstName", "HourlyRate", "LastName", "Password", "Role" },
                values: new object[] { 1, "hr@company.com", "Admin", 0, "HR", "HR123", 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserModel",
                keyColumn: "UserId",
                keyValue: 1);
        }
    }
}
