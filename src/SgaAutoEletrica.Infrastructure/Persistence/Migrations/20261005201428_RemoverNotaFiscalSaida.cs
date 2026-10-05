using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SgaAutoEletrica.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoverNotaFiscalSaida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensNotaFiscalSaida");

            migrationBuilder.DropTable(
                name: "NotasFiscaisSaida");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotasFiscaisSaida",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClienteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrdemServicoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    VeiculoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DataEmissao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ValorTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasFiscaisSaida", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotasFiscaisSaida_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotasFiscaisSaida_OrdensServico_OrdemServicoId",
                        column: x => x.OrdemServicoId,
                        principalTable: "OrdensServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_NotasFiscaisSaida_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItensNotaFiscalSaida",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NotaFiscalSaidaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensNotaFiscalSaida", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensNotaFiscalSaida_NotasFiscaisSaida_NotaFiscalSaidaId",
                        column: x => x.NotaFiscalSaidaId,
                        principalTable: "NotasFiscaisSaida",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItensNotaFiscalSaida_NotaFiscalSaidaId",
                table: "ItensNotaFiscalSaida",
                column: "NotaFiscalSaidaId");

            migrationBuilder.CreateIndex(
                name: "IX_NotasFiscaisSaida_ClienteId",
                table: "NotasFiscaisSaida",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_NotasFiscaisSaida_OrdemServicoId",
                table: "NotasFiscaisSaida",
                column: "OrdemServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_NotasFiscaisSaida_VeiculoId",
                table: "NotasFiscaisSaida",
                column: "VeiculoId");
        }
    }
}
