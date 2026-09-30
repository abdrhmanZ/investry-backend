using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investry.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationAndPromotionalVideoURLToProjectTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Projects",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromotionalVideoURL",
                table: "Projects",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PromotionalVideoURL",
                table: "Projects");
        }
    }
}
