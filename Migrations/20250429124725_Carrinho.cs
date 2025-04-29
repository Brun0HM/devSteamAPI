using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace devSteamAPI.Migrations
{
    /// <inheritdoc />
    public partial class Carrinho : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateCriacao",
                table: "Carrinhos",
                newName: "DataCriacao");

            migrationBuilder.AddColumn<decimal>(
                name: "ValorTotal",
                table: "Carrinhos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValorTotal",
                table: "Carrinhos");

            migrationBuilder.RenameColumn(
                name: "DataCriacao",
                table: "Carrinhos",
                newName: "DateCriacao");
        }
    }
}
