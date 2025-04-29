using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace devSteamAPI.Migrations
{
    /// <inheritdoc />
    public partial class Preco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrecoOriginal",
                table: "Jogos",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecoOriginal",
                table: "Jogos");
        }
    }
}
