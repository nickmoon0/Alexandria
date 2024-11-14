using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alexandria.Api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SplitPersonsName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Person",
                newName: "MiddleNames");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Person",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Person",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Person");

            migrationBuilder.RenameColumn(
                name: "MiddleNames",
                table: "Person",
                newName: "Name");
        }
    }
}
