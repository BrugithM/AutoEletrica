using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SgaAutoEletrica.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarConfiguracaoEmpresa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracoesEmpresa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NomeEmpresa = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Cnpj = table.Column<string>(type: "TEXT", maxLength: 18, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Endereco = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    UltimaAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracoesEmpresa", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracoesEmpresa");
        }
    }
}
