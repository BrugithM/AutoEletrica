using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Application.Features.Usuarios.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Usuarios;

public class DesativarUsuarioHandler : IRequestHandler<DesativarUsuarioCommand>
{
    private readonly AppDbContext _context;
    private readonly ISessaoUsuario _sessao;

    public DesativarUsuarioHandler(AppDbContext context, ISessaoUsuario sessao)
    {
        _context = context;
        _sessao = sessao;
    }

    public async Task Handle(DesativarUsuarioCommand request, CancellationToken cancellationToken)
    {
        if (_sessao.UsuarioAtual?.Id == request.UsuarioId)
            throw new InvalidOperationException("Você não pode desativar seu próprio usuário.");

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == request.UsuarioId, cancellationToken)
            ?? throw new InvalidOperationException("Usuário não encontrado.");

        if (usuario.Nivel == Domain.Enums.NivelUsuario.Administrador)
        {
            var adminsAtivos = await _context.Usuarios
                .CountAsync(u => u.Nivel == Domain.Enums.NivelUsuario.Administrador && u.Ativo, cancellationToken);

            if (adminsAtivos <= 1)
                throw new InvalidOperationException("Não é possível desativar o último administrador ativo.");
        }

        usuario.Desativar();
        await _context.SaveChangesAsync(cancellationToken);
    }
}