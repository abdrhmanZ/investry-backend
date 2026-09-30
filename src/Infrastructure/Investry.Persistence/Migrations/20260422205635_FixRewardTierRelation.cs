using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investry.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixRewardTierRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Investments_RewardTiers_RewardTierId1",
                table: "Investments");

            migrationBuilder.DropIndex(
                name: "IX_Investments_RewardTierId1",
                table: "Investments");

            migrationBuilder.DropColumn(
                name: "RewardTierId1",
                table: "Investments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RewardTierId1",
                table: "Investments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Investments_RewardTierId1",
                table: "Investments",
                column: "RewardTierId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Investments_RewardTiers_RewardTierId1",
                table: "Investments",
                column: "RewardTierId1",
                principalTable: "RewardTiers",
                principalColumn: "Id");
        }
    }
}
