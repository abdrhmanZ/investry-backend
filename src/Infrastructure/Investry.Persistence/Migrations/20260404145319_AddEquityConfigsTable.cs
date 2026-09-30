using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investry.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEquityConfigsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Investments_RewardTiers_RewardTierId",
                table: "Investments");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Founders_FounderId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_RewardTiers_RewardConfigId",
                table: "RewardTiers");

            migrationBuilder.DropIndex(
                name: "IX_Investments_InvestorId",
                table: "Investments");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "RewardTiers",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "RewardTiers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Projects",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Projects",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "EquityConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyValuation = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EquityPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquityConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquityConfigs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvestorShare",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvestorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvestmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquityConfigId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AmountInvested = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SharesPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestorShare", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestorShare_EquityConfigs_EquityConfigId",
                        column: x => x.EquityConfigId,
                        principalTable: "EquityConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvestorShare_Investments_InvestmentId",
                        column: x => x.InvestmentId,
                        principalTable: "Investments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvestorShare_Investors_InvestorId",
                        column: x => x.InvestorId,
                        principalTable: "Investors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RewardTiers_RewardConfigId_Title",
                table: "RewardTiers",
                columns: new[] { "RewardConfigId", "Title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Investments_InvestorId_RewardTierId",
                table: "Investments",
                columns: new[] { "InvestorId", "RewardTierId" },
                unique: true,
                filter: "[RewardTierId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EquityConfigs_ProjectId",
                table: "EquityConfigs",
                column: "ProjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestorShare_EquityConfigId",
                table: "InvestorShare",
                column: "EquityConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestorShare_InvestmentId",
                table: "InvestorShare",
                column: "InvestmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestorShare_InvestorId",
                table: "InvestorShare",
                column: "InvestorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Investments_RewardTiers_RewardTierId",
                table: "Investments",
                column: "RewardTierId",
                principalTable: "RewardTiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Founders_FounderId",
                table: "Projects",
                column: "FounderId",
                principalTable: "Founders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Investments_RewardTiers_RewardTierId",
                table: "Investments");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Founders_FounderId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "InvestorShare");

            migrationBuilder.DropTable(
                name: "EquityConfigs");

            migrationBuilder.DropIndex(
                name: "IX_RewardTiers_RewardConfigId_Title",
                table: "RewardTiers");

            migrationBuilder.DropIndex(
                name: "IX_Investments_InvestorId_RewardTierId",
                table: "Investments");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "RewardTiers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "RewardTiers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Projects",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateIndex(
                name: "IX_RewardTiers_RewardConfigId",
                table: "RewardTiers",
                column: "RewardConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Investments_InvestorId",
                table: "Investments",
                column: "InvestorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Investments_RewardTiers_RewardTierId",
                table: "Investments",
                column: "RewardTierId",
                principalTable: "RewardTiers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Founders_FounderId",
                table: "Projects",
                column: "FounderId",
                principalTable: "Founders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
