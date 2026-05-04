using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CyberFork.Migrations
{
    /// <inheritdoc />
    public partial class MakeNameUniqueAndSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_dishes_name",
                table: "dishes",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_dishes_name",
                table: "dishes");
        }
    }
}
