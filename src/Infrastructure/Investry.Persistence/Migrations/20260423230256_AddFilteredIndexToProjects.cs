using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investry.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFilteredIndexToProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_Title_FounderId",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Title_FounderId",
                table: "Projects",
                columns: new[] { "Title", "FounderId" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_Title_FounderId",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Title_FounderId",
                table: "Projects",
                columns: new[] { "Title", "FounderId" },
                unique: true);
        }
    }
}
