using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Application.Features.Usuarios.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Usuarios;

public class AlterarMeuNomeHandler : IRequestHandler<AlterarMeuNomeCommand>
{
    private readonly AppDbContext _context;
    private readonly ISessaoUsuario _sessao;

    public AlterarMeuNomeHandler(AppDbContext context, ISessaoUsuario sessao)
    {
        _context = context;
        _sessao = sessao;
    }

    public async Task Handle(AlterarMeuNomeCommand request, CancellationToken cancellationToken)
    {
        if (_sessao.UsuarioAtual == null)
            throw new InvalidOperationException("Nenhum usuário logado.");

        if (string.IsNullOrWhiteSpace(request.NovoNome))
            throw new InvalidOperationException("Nome é obrigatório.");

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == _sessao.UsuarioAtual.Id, cancellationToken)
            ?? throw new InvalidOperationException("Usuário não encontrado.");

        var nomeEmUso = await _context.Usuarios
            .AnyAsync(u => u.Nome == request.NovoNome && u.Id != usuario.Id, cancellationToken);

        if (nomeEmUso)
            throw new InvalidOperationException($"Já existe um usuário com o nome '{request.NovoNome}'.");

        usuario.AlterarNome(request.NovoNome);
        await _context.SaveChangesAsync(cancellationToken);

        _sessao.DefinirUsuario(usuario);
    }
}