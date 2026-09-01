using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SgaAutoEletrica.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarConfiguracaoImpressora : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConficuracoesImpressora",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    NomeImpressora = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TamanhoPapel = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Copias = table.Column<int>(type: "INTEGER", nullable: true, defaultValue: 1),
                    MargemSuperior = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    MargemInferior = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    MargemEsquerda = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    MargemDireita = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConficuracoesImpressora", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConficuracoesImpressora_Tipo",
                table: "ConficuracoesImpressora",
                column: "Tipo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConficuracoesImpressora");
        }
    }
}
