using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.DTOs;
using SgaAutoEletrica.Application.Features.Clientes.DTOs;
using SgaAutoEletrica.Application.Features.Clientes.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Clientes;

public class ListarClientesHandler : IRequestHandler<ListarClientesQuery, ListaPaginadaDTO<ClienteDTO>>
{
    private readonly AppDbContext _context;

    public ListarClientesHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ListaPaginadaDTO<ClienteDTO>> Handle(ListarClientesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Clientes
            .Include(c => c.Veiculos)
            .AsNoTracking()
            .AsQueryable();

        // Busca rápida (procura em vários campos)
        if (!string.IsNullOrWhiteSpace(request.TermoBusca))
        {
            var termo = request.TermoBusca.Trim().ToLower();
            query = query.Where(c =>
                c.NomeCompleto.ToLower().Contains(termo) ||
                c.Cpf.Valor.Contains(termo) ||
                c.Telefone.Valor.Contains(termo));
        }

        // Busca avançada (cada campo individual)
        if (!string.IsNullOrWhiteSpace(request.Nome))
        {
            var termo = request.Nome.Trim().ToLower();
            query = query.Where(c => c.NomeCompleto.ToLower().Contains(termo));
        }

        if (!string.IsNullOrWhiteSpace(request.Cpf))
        {
            var termo = new string(request.Cpf.Where(char.IsDigit).ToArray());
            query = query.Where(c => c.Cpf.Valor.Contains(termo));
        }

        if (!string.IsNullOrWhiteSpace(request.Telefone))
        {
            var termo = new string(request.Telefone.Where(char.IsDigit).ToArray());
            query = query.Where(c => c.Telefone.Valor.Contains(termo));
        }

        if (!string.IsNullOrWhiteSpace(request.Endereco))
        {
            var termo = request.Endereco.Trim().ToLower();
            query = query.Where(c =>
                c.Endereco != null && (
                    c.Endereco.Logradouro.ToLower().Contains(termo) ||
                    c.Endereco.Bairro.ToLower().Contains(termo) ||
                    c.Endereco.Cidade.ToLower().Contains(termo) ||
                    c.Endereco.Cep.Contains(termo)));
        }

        // Filtro de ativo
        if (request.Ativo.HasValue)
            query = query.Where(c => c.Ativo == request.Ativo.Value);

        var totalItens = await query.CountAsync(cancellationToken);

        var itens = await query
            .OrderBy(c => c.NomeCompleto)
            .Skip((request.Pagina - 1) * request.TamanhoPagina)
            .Take(request.TamanhoPagina)
            .Select(c => new ClienteDTO
            {
                Id = c.Id,
                NomeCompleto = c.NomeCompleto,
                Cpf = c.Cpf.Formatado(),
                Telefone = c.Telefone.Formatado(),
                EnderecoCompleto = c.Endereco != null ? c.Endereco.Completo() : null,
                DataCadastro = c.DataCadastro,
                QuantidadeVeiculos = c.Veiculos.Count,
                Ativo = c.Ativo
            })
            .ToListAsync(cancellationToken);

        return new ListaPaginadaDTO<ClienteDTO>
        {
            Itens = itens,
            PaginaAtual = request.Pagina,
            TamanhoPagina = request.TamanhoPagina,
            TotalItens = totalItens
        };
    }
}