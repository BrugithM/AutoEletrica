using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SgaAutoEletrica.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MigracaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriasPecas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasPecas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NomeCompleto = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Cpf = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    EnderecoLogradouro = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    EnderecoNumero = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    EnderecoComplemento = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EnderecoBairro = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EnderecoCidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EnderecoEstado = table.Column<string>(type: "TEXT", maxLength: 2, nullable: true),
                    EnderecoCep = table.Column<string>(type: "TEXT", maxLength: 8, nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fornecedores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NomeEmpresa = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Cnpj = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 11, nullable: true),
                    EnderecoLogradouro = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    EnderecoNumero = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    EnderecoComplemento = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EnderecoBairro = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EnderecoCidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EnderecoEstado = table.Column<string>(type: "TEXT", maxLength: 2, nullable: true),
                    EnderecoCep = table.Column<string>(type: "TEXT", maxLength: 8, nullable: true),
                    Contato = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fornecedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Servicos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    PrecoPadrao = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Veiculos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Placa = table.Column<string>(type: "TEXT", maxLength: 7, nullable: false),
                    Modelo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Marca = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Ano = table.Column<int>(type: "INTEGER", nullable: false),
                    Versao = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Motor = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TipoMotor = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Cor = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ClienteId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veiculos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veiculos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotasFiscaisEntrada",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DataEntrada = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FornecedorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Finalizada = table.Column<bool>(type: "INTEGER", nullable: false),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasFiscaisEntrada", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotasFiscaisEntrada_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pecas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdPeca = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CodigoPeca = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CodigoBarras = table.Column<string>(type: "TEXT", maxLength: 48, nullable: true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Marca = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CategoriaId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ValorCusto = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ValorVenda = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Imposto = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Estoque = table.Column<int>(type: "INTEGER", nullable: false),
                    EstoqueMinimo = table.Column<int>(type: "INTEGER", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FornecedorId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pecas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pecas_CategoriasPecas_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "CategoriasPecas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pecas_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OrdensServico",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Numero = table.Column<int>(type: "INTEGER", nullable: false),
                    ClienteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    VeiculoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataFinalizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ValorTotalPecas = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ValorTotalServicos = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdensServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdensServico_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdensServico_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItensNotaFiscalEntrada",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NotaFiscalEntradaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PecaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensNotaFiscalEntrada", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensNotaFiscalEntrada_NotasFiscaisEntrada_NotaFiscalEntradaId",
                        column: x => x.NotaFiscalEntradaId,
                        principalTable: "NotasFiscaisEntrada",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItensNotaFiscalEntrada_Pecas_PecaId",
                        column: x => x.PecaId,
                        principalTable: "Pecas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItensPecaOS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrdemServicoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PecaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensPecaOS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensPecaOS_OrdensServico_OrdemServicoId",
                        column: x => x.OrdemServicoId,
                        principalTable: "OrdensServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItensPecaOS_Pecas_PecaId",
                        column: x => x.PecaId,
                        principalTable: "Pecas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItensServicoOS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrdemServicoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServicoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensServicoOS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensServicoOS_OrdensServico_OrdemServicoId",
                        column: x => x.OrdemServicoId,
                        principalTable: "OrdensServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItensServicoOS_Servicos_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotasFiscaisSaida",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DataEmissao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OrdemServicoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ClienteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    VeiculoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
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
                    ValorUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
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
                name: "IX_ItensNotaFiscalEntrada_NotaFiscalEntradaId",
                table: "ItensNotaFiscalEntrada",
                column: "NotaFiscalEntradaId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensNotaFiscalEntrada_PecaId",
                table: "ItensNotaFiscalEntrada",
                column: "PecaId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensNotaFiscalSaida_NotaFiscalSaidaId",
                table: "ItensNotaFiscalSaida",
                column: "NotaFiscalSaidaId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensPecaOS_OrdemServicoId",
                table: "ItensPecaOS",
                column: "OrdemServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensPecaOS_PecaId",
                table: "ItensPecaOS",
                column: "PecaId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensServicoOS_OrdemServicoId",
                table: "ItensServicoOS",
                column: "OrdemServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensServicoOS_ServicoId",
                table: "ItensServicoOS",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_NotasFiscaisEntrada_FornecedorId",
                table: "NotasFiscaisEntrada",
                column: "FornecedorId");

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

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_ClienteId",
                table: "OrdensServico",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_Numero",
                table: "OrdensServico",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_VeiculoId",
                table: "OrdensServico",
                column: "VeiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pecas_CategoriaId",
                table: "Pecas",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pecas_FornecedorId",
                table: "Pecas",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculos_ClienteId",
                table: "Veiculos",
                column: "ClienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensNotaFiscalEntrada");

            migrationBuilder.DropTable(
                name: "ItensNotaFiscalSaida");

            migrationBuilder.DropTable(
                name: "ItensPecaOS");

            migrationBuilder.DropTable(
                name: "ItensServicoOS");

            migrationBuilder.DropTable(
                name: "NotasFiscaisEntrada");

            migrationBuilder.DropTable(
                name: "NotasFiscaisSaida");

            migrationBuilder.DropTable(
                name: "Pecas");

            migrationBuilder.DropTable(
                name: "Servicos");

            migrationBuilder.DropTable(
                name: "OrdensServico");

            migrationBuilder.DropTable(
                name: "CategoriasPecas");

            migrationBuilder.DropTable(
                name: "Fornecedores");

            migrationBuilder.DropTable(
                name: "Veiculos");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
