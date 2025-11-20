using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG6212POE.Migrations
{
    /// <inheritdoc />
    public partial class mssqllocal_migration_623 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClaimModel",
                columns: table => new
                {
                    ClaimId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    ClaimStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LecturerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuppDoc = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimModel", x => x.ClaimId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClaimModel");
        }
    }
}
