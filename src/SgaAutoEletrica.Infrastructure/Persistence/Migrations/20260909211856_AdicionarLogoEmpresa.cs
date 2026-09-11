using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SgaAutoEletrica.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarLogoEmpresa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoPath",
                table: "ConfiguracoesEmpresa",
                type: "TEXT",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoPath",
                table: "ConfiguracoesEmpresa");
        }
    }
}
