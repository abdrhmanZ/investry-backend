using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investry.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LinkFounderProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Projects",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "FounderId",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Projects_FounderId",
                table: "Projects",
                column: "FounderId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Title_FounderId",
                table: "Projects",
                columns: new[] { "Title", "FounderId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Founders_FounderId",
                table: "Projects",
                column: "FounderId",
                principalTable: "Founders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Founders_FounderId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_FounderId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_Title_FounderId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "FounderId",
                table: "Projects");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
