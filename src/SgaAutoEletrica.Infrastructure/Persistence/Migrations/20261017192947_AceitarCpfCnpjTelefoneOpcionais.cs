using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SgaAutoEletrica.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AceitarCpfCnpjTelefoneOpcionais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cpf",
                table: "Clientes",
                newName: "CpfCnpj");

            migrationBuilder.Sql("UPDATE Clientes SET Telefone = NULL WHERE Telefone = '';");

            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "Clientes",
                type: "TEXT",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 11);

            migrationBuilder.AlterColumn<string>(
                name: "CpfCnpj",
                table: "Clientes",
                type: "TEXT",
                maxLength: 14,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 11);

            migrationBuilder.AddColumn<bool>(
                name: "EhCpf",
                table: "Clientes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE Clientes
                SET EhCpf = CASE
                    WHEN LENGTH(CpfCnpj) = 11 THEN 1
                    WHEN LENGTH(CpfCnpj) = 14 THEN 0
                    ELSE NULL
                END
                WHERE CpfCnpj IS NOT NULL AND CpfCnpj <> '';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EhCpf",
                table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "CpfCnpj",
                table: "Clientes",
                newName: "Cpf");

            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "Clientes",
                type: "TEXT",
                maxLength: 11,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cpf",
                table: "Clientes",
                type: "TEXT",
                maxLength: 11,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 14,
                oldNullable: true);
        }
    }
}
