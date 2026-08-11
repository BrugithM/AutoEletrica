using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Domain.Enums;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (context.Clientes.Any())
        {
            Console.WriteLine("  Dados já existem. Pulando seed.");
            return;
        }

        // ─── Clientes ───
        var cliente1 = new Cliente("João Silva", "529.982.247-25", "11999990000");
        cliente1.AdicionarEndereco(new("Rua das Flores", "Centro", "São Paulo", "SP", "01234567", "123"));
        context.Clientes.Add(cliente1);

        var cliente2 = new Cliente("Maria Oliveira", "170.477.383-04", "21988887777");
        context.Clientes.Add(cliente2);

        await context.SaveChangesAsync();
        Console.WriteLine("Clientes salvos.");

        // ─── Veículos ───
        var veiculo1 = new Veiculo("ABC1D23", cliente1.Id);
        veiculo1.PreencherDadosViaApi("Gol", "Volkswagen", 2020, "Comfortline", "1.0 TSI", TipoMotor.Flex, "Prata");
        context.Veiculos.Add(veiculo1);

        var veiculo2 = new Veiculo("DEF4G56", cliente2.Id, "Fiesta", "Ford", 2019);
        veiculo2.AtualizarDados("Fiesta", "Ford", 2019, "SE", "1.6", TipoMotor.Flex, "Preto");
        context.Veiculos.Add(veiculo2);

        await context.SaveChangesAsync();
        Console.WriteLine("Veículos salvos.");

        // ─── Fornecedor ───
        var fornecedor = new Fornecedor("Auto Peças Silva", "13.599.816/0001-25");
        fornecedor.AtualizarDados("Auto Peças Silva", "1133334444", "Carlos Silva");
        context.Fornecedores.Add(fornecedor);

        await context.SaveChangesAsync();
        Console.WriteLine("Fornecedor salvo.");

        // ─── Categorias ───
        var categoriaMotor = new CategoriaPeca("Motor", "Peças de motor em geral");
        context.CategoriasPecas.Add(categoriaMotor);

        var categoriaFreios = new CategoriaPeca("Freios", "Sistema de frenagem");
        context.CategoriasPecas.Add(categoriaFreios);

        await context.SaveChangesAsync();
        Console.WriteLine("Categorias salvas.");

        // ─── Peças ───
        var peca1 = new Peca(
            "P-0001", "Pastilha de Freio Dianteira", "Pastilha dianteira Gol G5",
            "Cobreq", 45.90m, 89.90m, 18m, 20,
            fornecedorId: fornecedor.Id, categoriaId: categoriaFreios.Id,
            codigoPeca: "FR-001", codigoBarras: "7891234567890");

        var peca2 = new Peca(
            "P-0002", "Filtro de Óleo", "Filtro de óleo motor AP 1.0/1.6",
            "Mann", 12.50m, 25.00m, 18m, 50,
            fornecedorId: fornecedor.Id, categoriaId: categoriaMotor.Id,
            codigoPeca: "MT-002", codigoBarras: "7890987654321");

        context.Pecas.Add(peca1);
        context.Pecas.Add(peca2);

        await context.SaveChangesAsync();
        Console.WriteLine("Peças salvas.");

        // ─── Serviços ───
        var servico1 = new Servico("Troca de Pastilha de Freio", 80.00m, "Troca do par dianteiro");
        var servico2 = new Servico("Troca de Óleo", 50.00m, "Inclui filtro de óleo");

        context.Servicos.Add(servico1);
        context.Servicos.Add(servico2);

        await context.SaveChangesAsync();
        Console.WriteLine("Serviços salvos.");

        // ─── Ordem de Serviço ───
        var os = new OrdemServico(1, cliente1.Id, veiculo1.Id, "Cliente relatou barulho na freagem");
        os.AdicionarPeca(peca1.Id, 1, peca1.ValorVenda);
        os.AdicionarServico(servico1.Id, servico1.PrecoPadrao);
        os.IniciarServico();
        os.Finalizar();

        context.OrdensServico.Add(os);
        
        await context.SaveChangesAsync();
        Console.WriteLine("OS salva.");

        Console.WriteLine("Dados de teste inseridos com sucesso!");
    }
}