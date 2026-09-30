using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investry.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMudarabahSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MudarabahConfig_Projects_ProjectId",
                table: "MudarabahConfig");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MudarabahConfig",
                table: "MudarabahConfig");

            migrationBuilder.RenameTable(
                name: "MudarabahConfig",
                newName: "MudarabahConfigs");

            migrationBuilder.RenameIndex(
                name: "IX_MudarabahConfig_ProjectId",
                table: "MudarabahConfigs",
                newName: "IX_MudarabahConfigs_ProjectId");

            migrationBuilder.AddColumn<DateTime>(
                name: "MudarabahEndDate",
                table: "MudarabahConfigs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MudarabahStartDate",
                table: "MudarabahConfigs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MudarabahConfigs",
                table: "MudarabahConfigs",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CapitalReturn",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvestorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvestmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MudarabahConfigId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReturnedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CapitalReturnStatus = table.Column<int>(type: "int", nullable: false),
                    ReturnedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapitalReturn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CapitalReturn_Investments_InvestmentId",
                        column: x => x.InvestmentId,
                        principalTable: "Investments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CapitalReturn_Investors_InvestorId",
                        column: x => x.InvestorId,
                        principalTable: "Investors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CapitalReturn_MudarabahConfigs_MudarabahConfigId",
                        column: x => x.MudarabahConfigId,
                        principalTable: "MudarabahConfigs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProfitDistributions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MudarabahConfigId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NetProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InvestorsPoolProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FounderProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DistributionStatus = table.Column<int>(type: "int", nullable: false),
                    DistributedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfitDistributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfitDistributions_MudarabahConfigs_MudarabahConfigId",
                        column: x => x.MudarabahConfigId,
                        principalTable: "MudarabahConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvestorProfitAllocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfitDistributionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvestorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvestmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CapitalRatio = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    AllocatedProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestorProfitAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestorProfitAllocations_Investments_InvestmentId",
                        column: x => x.InvestmentId,
                        principalTable: "Investments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvestorProfitAllocations_Investors_InvestorId",
                        column: x => x.InvestorId,
                        principalTable: "Investors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvestorProfitAllocations_ProfitDistributions_ProfitDistributionId",
                        column: x => x.ProfitDistributionId,
                        principalTable: "ProfitDistributions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CapitalReturn_CapitalReturnStatus",
                table: "CapitalReturn",
                column: "CapitalReturnStatus");

            migrationBuilder.CreateIndex(
                name: "IX_CapitalReturn_InvestmentId",
                table: "CapitalReturn",
                column: "InvestmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CapitalReturn_InvestorId",
                table: "CapitalReturn",
                column: "InvestorId");

            migrationBuilder.CreateIndex(
                name: "IX_CapitalReturn_MudarabahConfigId",
                table: "CapitalReturn",
                column: "MudarabahConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestorProfitAllocations_InvestmentId",
                table: "InvestorProfitAllocations",
                column: "InvestmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestorProfitAllocations_InvestorId",
                table: "InvestorProfitAllocations",
                column: "InvestorId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestorProfitAllocations_ProfitDistributionId",
                table: "InvestorProfitAllocations",
                column: "ProfitDistributionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfitDistributions_CreatedAt",
                table: "ProfitDistributions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProfitDistributions_DistributionStatus",
                table: "ProfitDistributions",
                column: "DistributionStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ProfitDistributions_MudarabahConfigId",
                table: "ProfitDistributions",
                column: "MudarabahConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_MudarabahConfigs_Projects_ProjectId",
                table: "MudarabahConfigs",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MudarabahConfigs_Projects_ProjectId",
                table: "MudarabahConfigs");

            migrationBuilder.DropTable(
                name: "CapitalReturn");

            migrationBuilder.DropTable(
                name: "InvestorProfitAllocations");

            migrationBuilder.DropTable(
                name: "ProfitDistributions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MudarabahConfigs",
                table: "MudarabahConfigs");

            migrationBuilder.DropColumn(
                name: "MudarabahEndDate",
                table: "MudarabahConfigs");

            migrationBuilder.DropColumn(
                name: "MudarabahStartDate",
                table: "MudarabahConfigs");

            migrationBuilder.RenameTable(
                name: "MudarabahConfigs",
                newName: "MudarabahConfig");

            migrationBuilder.RenameIndex(
                name: "IX_MudarabahConfigs_ProjectId",
                table: "MudarabahConfig",
                newName: "IX_MudarabahConfig_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MudarabahConfig",
                table: "MudarabahConfig",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MudarabahConfig_Projects_ProjectId",
                table: "MudarabahConfig",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
