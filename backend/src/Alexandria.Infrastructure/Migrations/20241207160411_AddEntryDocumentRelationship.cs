using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alexandria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEntryDocumentRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EntryId",
                table: "Document",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Document_EntryId",
                table: "Document",
                column: "EntryId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Document_Entry_EntryId",
                table: "Document",
                column: "EntryId",
                principalTable: "Entry",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Document_Entry_EntryId",
                table: "Document");

            migrationBuilder.DropIndex(
                name: "IX_Document_EntryId",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "EntryId",
                table: "Document");
        }
    }
}
