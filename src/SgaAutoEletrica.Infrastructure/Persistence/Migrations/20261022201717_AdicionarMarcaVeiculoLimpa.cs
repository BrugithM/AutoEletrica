using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SgaAutoEletrica.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarMarcaVeiculoLimpa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MarcaVeiculoId",
                table: "Veiculos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MarcasVeiculo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarcasVeiculo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Veiculos_MarcaVeiculoId",
                table: "Veiculos",
                column: "MarcaVeiculoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculos_MarcasVeiculo_MarcaVeiculoId",
                table: "Veiculos",
                column: "MarcaVeiculoId",
                principalTable: "MarcasVeiculo",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.Sql(@"
                INSERT INTO MarcasVeiculo (Id, Nome, Ativo)
                SELECT 
                    upper(hex(randomblob(4))) || '-' || upper(hex(randomblob(2))) || '-4' || substr(upper(hex(randomblob(2))),2) || '-' || substr('89AB',abs(random()) % 4 + 1, 1) || substr(upper(hex(randomblob(2))),2) || '-' || upper(hex(randomblob(6))),
                    UPPER(Marca), 
                    1
                FROM Veiculos
                WHERE Marca IS NOT NULL AND TRIM(Marca) <> ''
                GROUP BY UPPER(Marca);
            ");

            migrationBuilder.Sql(@"
                UPDATE Veiculos
                SET MarcaVeiculoId = (
                    SELECT Id FROM MarcasVeiculo 
                    WHERE MarcasVeiculo.Nome = UPPER(Veiculos.Marca)
                    LIMIT 1
                )
                WHERE Marca IS NOT NULL AND TRIM(Marca) <> '';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Veiculos_MarcasVeiculo_MarcaVeiculoId",
                table: "Veiculos");

            migrationBuilder.DropTable(
                name: "MarcasVeiculo");

            migrationBuilder.DropIndex(
                name: "IX_Veiculos_MarcaVeiculoId",
                table: "Veiculos");

            migrationBuilder.DropColumn(
                name: "MarcaVeiculoId",
                table: "Veiculos");
        }
    }
}