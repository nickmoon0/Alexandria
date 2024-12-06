using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alexandria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tagging",
                columns: table => new
                {
                    TaggingId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TagId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EntityType = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tagging", x => x.TaggingId);
                    table.ForeignKey(
                        name: "FK_Tagging_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_Name",
                table: "Tag",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tagging_EntityType_EntityId",
                table: "Tagging",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Tagging_TagId",
                table: "Tagging",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tagging");

            migrationBuilder.DropTable(
                name: "Tag");
        }
    }
}
