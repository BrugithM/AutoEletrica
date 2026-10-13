using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Usuarios.Commands;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Usuarios;

public class ResetarSenhaUsuarioHandler : IRequestHandler<ResetarSenhaUsuarioCommand>
{
    private readonly AppDbContext _context;

    public ResetarSenhaUsuarioHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ResetarSenhaUsuarioCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NovaSenha) || request.NovaSenha.Length < 6)
            throw new InvalidOperationException("Senha deve ter pelo menos 6 caracteres.");

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == request.UsuarioId, cancellationToken)
            ?? throw new InvalidOperationException("Usuário não encontrado.");

        var novoHash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);
        usuario.AlterarSenha(novoHash);
        await _context.SaveChangesAsync(cancellationToken);
    }
}