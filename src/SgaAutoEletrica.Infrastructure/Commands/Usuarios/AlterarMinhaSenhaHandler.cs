using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Application.Features.Usuarios.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Usuarios;

public class AlterarMinhaSenhaHandler : IRequestHandler<AlterarMinhaSenhaCommand>
{
    private readonly AppDbContext _context;
    private readonly ISessaoUsuario _sessao;

    public AlterarMinhaSenhaHandler(AppDbContext context, ISessaoUsuario sessao)
    {
        _context = context;
        _sessao = sessao;
    }

    public async Task Handle(AlterarMinhaSenhaCommand request, CancellationToken cancellationToken)
    {
        if (_sessao.UsuarioAtual == null)
            throw new InvalidOperationException("Nenhum usuário logado.");

        if (string.IsNullOrWhiteSpace(request.NovaSenha) || request.NovaSenha.Length < 6)
            throw new InvalidOperationException("Nova senha deve ter pelo menos 6 caracteres.");

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == _sessao.UsuarioAtual.Id, cancellationToken)
            ?? throw new InvalidOperationException("Usuário não encontrado.");

        if (!BCrypt.Net.BCrypt.Verify(request.SenhaAtual, usuario.SenhaHash))
            throw new InvalidOperationException("Senha atual incorreta.");

        var novoHash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);
        usuario.AlterarSenha(novoHash);
        await _context.SaveChangesAsync(cancellationToken);

        _sessao.DefinirUsuario(usuario);
    }
}