using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investry.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixWalletRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Wallets");

            migrationBuilder.RenameColumn(
                name: "StripeSessionId",
                table: "WalletTransactions",
                newName: "SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_WalletTransactions_StripeSessionId",
                table: "WalletTransactions",
                newName: "IX_WalletTransactions_SessionId");

            migrationBuilder.AddColumn<Guid>(
                name: "FounderId",
                table: "Wallets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InvestorId",
                table: "Wallets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_FounderId",
                table: "Wallets",
                column: "FounderId",
                unique: true,
                filter: "[FounderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_InvestorId",
                table: "Wallets",
                column: "InvestorId",
                unique: true,
                filter: "[InvestorId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Founders_FounderId",
                table: "Wallets",
                column: "FounderId",
                principalTable: "Founders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Investors_InvestorId",
                table: "Wallets",
                column: "InvestorId",
                principalTable: "Investors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Founders_FounderId",
                table: "Wallets");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Investors_InvestorId",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_FounderId",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_InvestorId",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "FounderId",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "InvestorId",
                table: "Wallets");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "WalletTransactions",
                newName: "StripeSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_WalletTransactions_SessionId",
                table: "WalletTransactions",
                newName: "IX_WalletTransactions_StripeSessionId");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Wallets",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets",
                column: "UserId",
                unique: true);
        }
    }
}
