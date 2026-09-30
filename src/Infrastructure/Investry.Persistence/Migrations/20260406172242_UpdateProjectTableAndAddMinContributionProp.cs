using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investry.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProjectTableAndAddMinContributionProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestorShare_EquityConfigs_EquityConfigId",
                table: "InvestorShare");

            migrationBuilder.DropForeignKey(
                name: "FK_InvestorShare_Investments_InvestmentId",
                table: "InvestorShare");

            migrationBuilder.DropForeignKey(
                name: "FK_InvestorShare_Investors_InvestorId",
                table: "InvestorShare");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvestorShare",
                table: "InvestorShare");

            migrationBuilder.DropColumn(
                name: "MinContribution",
                table: "RewardConfigs");

            migrationBuilder.DropColumn(
                name: "CompanyValuation",
                table: "EquityConfigs");

            migrationBuilder.RenameTable(
                name: "InvestorShare",
                newName: "InvestorShares");

            migrationBuilder.RenameIndex(
                name: "IX_InvestorShare_InvestorId",
                table: "InvestorShares",
                newName: "IX_InvestorShares_InvestorId");

            migrationBuilder.RenameIndex(
                name: "IX_InvestorShare_InvestmentId",
                table: "InvestorShares",
                newName: "IX_InvestorShares_InvestmentId");

            migrationBuilder.RenameIndex(
                name: "IX_InvestorShare_EquityConfigId",
                table: "InvestorShares",
                newName: "IX_InvestorShares_EquityConfigId");

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumContribution",
                table: "Projects",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvestorShares",
                table: "InvestorShares",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvestorShares_EquityConfigs_EquityConfigId",
                table: "InvestorShares",
                column: "EquityConfigId",
                principalTable: "EquityConfigs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvestorShares_Investments_InvestmentId",
                table: "InvestorShares",
                column: "InvestmentId",
                principalTable: "Investments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvestorShares_Investors_InvestorId",
                table: "InvestorShares",
                column: "InvestorId",
                principalTable: "Investors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestorShares_EquityConfigs_EquityConfigId",
                table: "InvestorShares");

            migrationBuilder.DropForeignKey(
                name: "FK_InvestorShares_Investments_InvestmentId",
                table: "InvestorShares");

            migrationBuilder.DropForeignKey(
                name: "FK_InvestorShares_Investors_InvestorId",
                table: "InvestorShares");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvestorShares",
                table: "InvestorShares");

            migrationBuilder.DropColumn(
                name: "MinimumContribution",
                table: "Projects");

            migrationBuilder.RenameTable(
                name: "InvestorShares",
                newName: "InvestorShare");

            migrationBuilder.RenameIndex(
                name: "IX_InvestorShares_InvestorId",
                table: "InvestorShare",
                newName: "IX_InvestorShare_InvestorId");

            migrationBuilder.RenameIndex(
                name: "IX_InvestorShares_InvestmentId",
                table: "InvestorShare",
                newName: "IX_InvestorShare_InvestmentId");

            migrationBuilder.RenameIndex(
                name: "IX_InvestorShares_EquityConfigId",
                table: "InvestorShare",
                newName: "IX_InvestorShare_EquityConfigId");

            migrationBuilder.AddColumn<decimal>(
                name: "MinContribution",
                table: "RewardConfigs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CompanyValuation",
                table: "EquityConfigs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvestorShare",
                table: "InvestorShare",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvestorShare_EquityConfigs_EquityConfigId",
                table: "InvestorShare",
                column: "EquityConfigId",
                principalTable: "EquityConfigs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvestorShare_Investments_InvestmentId",
                table: "InvestorShare",
                column: "InvestmentId",
                principalTable: "Investments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvestorShare_Investors_InvestorId",
                table: "InvestorShare",
                column: "InvestorId",
                principalTable: "Investors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
