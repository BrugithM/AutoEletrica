using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SgaAutoEletrica.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarAtivoFornecedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Fornecedores",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Fornecedores");
        }
    }
}
