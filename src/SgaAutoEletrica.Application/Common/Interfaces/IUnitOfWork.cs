namespace SgaAutoEletrica.Application.Common.Interfaces;

//Padrão Unit of Work - atomicidade das operações
//"tudo ou nada" ou tudo da certo ou nada é feito

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}