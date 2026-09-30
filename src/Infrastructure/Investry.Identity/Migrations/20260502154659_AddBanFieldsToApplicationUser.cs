using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investry.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddBanFieldsToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BanExpiry",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BanReason",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "BanExpiry", "BanReason", "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { null, null, "f47af2a4-98b6-4b5a-bb8d-1573cdf270ae", new DateTime(2026, 5, 2, 15, 46, 57, 917, DateTimeKind.Utc).AddTicks(1682), "", "23321179-8f91-4f4a-8975-610bdfdb06e3" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "BanExpiry", "BanReason", "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { null, null, "c945932b-7fde-4383-b23a-158b8ba2a93f", new DateTime(2026, 5, 2, 15, 46, 58, 20, DateTimeKind.Utc).AddTicks(2222), "", "5330432d-546c-4b6e-b6b8-e5bced614ff7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e4a1c9b2-6f3d-4a8e-9c71-1b5f8d2a0e34",
                columns: new[] { "BanExpiry", "BanReason", "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { null, null, "ab91de0a-1606-4e37-8948-4ac9b0bdc9fe", new DateTime(2026, 5, 2, 15, 46, 58, 138, DateTimeKind.Utc).AddTicks(3597), "", "6da0b412-e869-4a49-9714-283332ed4c3a" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BanExpiry",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BanReason",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "516d407d-e072-4a82-94a1-ff955215946e", new DateTime(2026, 4, 19, 14, 38, 55, 241, DateTimeKind.Utc).AddTicks(5813), "", "5d46689d-b05a-40c3-989e-c23a8da2b200" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ce358fb6-2dd3-46c7-b2c9-ff91b5f6c592", new DateTime(2026, 4, 19, 14, 38, 55, 322, DateTimeKind.Utc).AddTicks(5938), "", "9bcce921-68ab-41aa-a724-f6450997b776" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e4a1c9b2-6f3d-4a8e-9c71-1b5f8d2a0e34",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "619812ad-f189-4e7c-b017-a9a26d72dae0", new DateTime(2026, 4, 19, 14, 38, 55, 404, DateTimeKind.Utc).AddTicks(2062), "", "71a12375-43a4-49a7-93c1-0724cc04a986" });
        }
    }
}
