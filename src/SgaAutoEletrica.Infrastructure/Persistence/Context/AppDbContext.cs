using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Domain.Entities;

namespace SgaAutoEletrica.Infrastructure.Persistence.Context;

///<summary>
/// Conexão com o banco de dados e gerenciamento de entidades
/// </summary>

public class AppDbContext : DbContext
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Veiculo> Veiculos => Set<Veiculo>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<CategoriaPeca> CategoriasPecas => Set<CategoriaPeca>();
    public DbSet<Peca> Pecas => Set<Peca>();
    public DbSet<Servico> Servicos => Set<Servico>();
    public DbSet<OrdemServico> OrdensServico => Set<OrdemServico>();
    public DbSet<ItemPecaOS> ItensPecaOS => Set<ItemPecaOS>();
    public DbSet<ItemServicoOS> ItensServicoOS => Set<ItemServicoOS>();
    public DbSet<NotaFiscalEntrada> NotasFiscaisEntrada => Set<NotaFiscalEntrada>();
    public DbSet<ItemNotaFiscalEntrada> ItensNotaFiscalEntrada  => Set<ItemNotaFiscalEntrada>();
    public DbSet<NotaFiscalSaida> NotasFiscaisSaida => Set<NotaFiscalSaida>();
    public DbSet<ItemNotaFiscalSaida> ItensNotaFiscalSaida => Set<ItemNotaFiscalSaida>();
    public DbSet<ConfiguracaoImpressora> ConfiguracoesImpressora => Set<ConfiguracaoImpressora>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<ConfiguracaoEmpresa> ConfiguracoesEmpresa => Set<ConfiguracaoEmpresa>();
    
    public AppDbContext(DbContextOptions<AppDbContext>options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}