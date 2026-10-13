using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Usuarios.DTOs;
using SgaAutoEletrica.Application.Features.Usuarios.Queries;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Queries.Usuarios;

public class ListarUsuariosHandler : IRequestHandler<ListarUsuariosQuery, List<UsuarioDTO>>
{
    private readonly AppDbContext _context;

    public ListarUsuariosHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UsuarioDTO>> Handle(ListarUsuariosQuery request, CancellationToken cancellationToken)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .OrderBy(u => u.Nome)
            .Select(u => new UsuarioDTO
            {
                Id = u.Id,
                Nome = u.Nome,
                Nivel = u.Nivel,
                Ativo = u.Ativo,
                DataCriacao = u.DataCriacao,
                UltimoLogin = u.UltimoLogin
            })
            .ToListAsync(cancellationToken);
    }
}