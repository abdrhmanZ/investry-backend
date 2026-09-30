using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Investry.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryProject",
                columns: table => new
                {
                    CategoriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryProject", x => new { x.CategoriesId, x.ProjectsId });
                    table.ForeignKey(
                        name: "FK_CategoryProject_Category_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryProject_Projects_ProjectsId",
                        column: x => x.ProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"), "Creative projects including film, music, design, and visual arts.", "Art & Culture" },
                    { new Guid("b1c2d3e4-f5a6-b7c8-d9e0-f1a2b3c4d5e6"), "Restaurants, food products, and innovative culinary ventures.", "Food & Beverage" },
                    { new Guid("c1d5e8f0-1a2b-3c4d-5e6f-7a8b9c0d1e2f"), "Projects in the medical, wellness, and health tech fields.", "Healthcare" },
                    { new Guid("d1e2f3a4-b5c6-d7e8-f9a0-1b2c3d4e5f6a"), "Educational tools, platforms, and learning resources.", "Education" },
                    { new Guid("e1f2a3b4-c5d6-e7f8-a9b0-1c2d3e4f5a6b"), "Projects focused on sustainability, conservation, and green technology.", "Environment" },
                    { new Guid("f1a2b3c4-d5e6-f7a8-9b0c-1d2e3f4a5b6c"), "Clothing, accessories, and innovative fashion-related projects.", "Fashion" },
                    { new Guid("f9f0a28a-723a-4a6c-9a4a-2e38c3e3e3e3"), "Projects related to software, hardware, and internet services.", "Technology" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Category_Name",
                table: "Category",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryProject_ProjectsId",
                table: "CategoryProject",
                column: "ProjectsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryProject");

            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
