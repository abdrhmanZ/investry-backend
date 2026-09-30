using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investry.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddLastLoginToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "LastLogin", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b39f3041-4c54-48dd-974c-3f75444e9eb8", new DateTime(2026, 5, 2, 17, 20, 26, 487, DateTimeKind.Utc).AddTicks(3126), null, "", "24b5a3cf-2d3f-4ed7-b246-d2f5d0445660" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "LastLogin", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cbc2c956-5b9e-4e15-b327-f4fc4ebd3520", new DateTime(2026, 5, 2, 17, 20, 26, 600, DateTimeKind.Utc).AddTicks(6933), null, "", "609fe3c1-b3fe-4a9f-9589-a997cd72cb45" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e4a1c9b2-6f3d-4a8e-9c71-1b5f8d2a0e34",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "LastLogin", "PasswordHash", "SecurityStamp" },
                values: new object[] { "23666021-5e87-4246-a74d-dcc2515b8c63", new DateTime(2026, 5, 2, 17, 20, 26, 782, DateTimeKind.Utc).AddTicks(2827), null, "", "15a3c0b4-3116-4fd7-921d-bee3dbf705cd" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f47af2a4-98b6-4b5a-bb8d-1573cdf270ae", new DateTime(2026, 5, 2, 15, 46, 57, 917, DateTimeKind.Utc).AddTicks(1682), "", "23321179-8f91-4f4a-8975-610bdfdb06e3" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c945932b-7fde-4383-b23a-158b8ba2a93f", new DateTime(2026, 5, 2, 15, 46, 58, 20, DateTimeKind.Utc).AddTicks(2222), "", "5330432d-546c-4b6e-b6b8-e5bced614ff7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e4a1c9b2-6f3d-4a8e-9c71-1b5f8d2a0e34",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ab91de0a-1606-4e37-8948-4ac9b0bdc9fe", new DateTime(2026, 5, 2, 15, 46, 58, 138, DateTimeKind.Utc).AddTicks(3597), "", "6da0b412-e869-4a49-9714-283332ed4c3a" });
        }
    }
}
