using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Infrastructure.Persistence.Repositories;

namespace SgaAutoEletrica.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICategoriaPecaRepository, CategoriaPecaRepository>();
        services.AddScoped<IServicoRepository, ServicoRepository>();
        services.AddScoped<IFornecedorRepository, FornecedorRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IVeiculoRepository, VeiculoRepository>();
        services.AddScoped<IPecaRepository, PecaRepository>();
        services.AddScoped<IOrdemServicoRepository, OrdemServicoRepository>();

        return services;
    }
}