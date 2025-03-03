using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alexandria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedTagEntityUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tagging_TagId_EntityId",
                table: "Tagging");

            migrationBuilder.CreateIndex(
                name: "IX_Tagging_TagId_EntityId",
                table: "Tagging",
                columns: new[] { "TagId", "EntityId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tagging_TagId_EntityId",
                table: "Tagging");

            migrationBuilder.CreateIndex(
                name: "IX_Tagging_TagId_EntityId",
                table: "Tagging",
                columns: new[] { "TagId", "EntityId" },
                unique: true);
        }
    }
}
