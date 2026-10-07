using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SgaAutoEletrica.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TrocarIdPecaParaIntEAdicionarMarca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdPeca",
                table: "Pecas");

            migrationBuilder.DropColumn(
                name: "Imposto",
                table: "Pecas");

            migrationBuilder.DropColumn(
                name: "Marca",
                table: "Pecas");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Pecas",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "MarcaId",
                table: "Pecas",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MarkupPercentual",
                table: "Pecas",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DescontoPercentual",
                table: "OrdensServico",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Quilometragem",
                table: "OrdensServico",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PecaId",
                table: "ItensPecaOS",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "PecaId",
                table: "ItensNotaFiscalEntrada",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "Marcas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marcas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pecas_MarcaId",
                table: "Pecas",
                column: "MarcaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pecas_Marcas_MarcaId",
                table: "Pecas",
                column: "MarcaId",
                principalTable: "Marcas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pecas_Marcas_MarcaId",
                table: "Pecas");

            migrationBuilder.DropTable(
                name: "Marcas");

            migrationBuilder.DropIndex(
                name: "IX_Pecas_MarcaId",
                table: "Pecas");

            migrationBuilder.DropColumn(
                name: "MarcaId",
                table: "Pecas");

            migrationBuilder.DropColumn(
                name: "MarkupPercentual",
                table: "Pecas");

            migrationBuilder.DropColumn(
                name: "DescontoPercentual",
                table: "OrdensServico");

            migrationBuilder.DropColumn(
                name: "Quilometragem",
                table: "OrdensServico");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Pecas",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "IdPeca",
                table: "Pecas",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Imposto",
                table: "Pecas",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Marca",
                table: "Pecas",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "PecaId",
                table: "ItensPecaOS",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "PecaId",
                table: "ItensNotaFiscalEntrada",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }
    }
}
