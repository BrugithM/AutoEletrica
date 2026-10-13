using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Usuarios.Commands;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Usuarios;

public class CriarUsuarioHandler : IRequestHandler<CriarUsuarioCommand, Guid>
{
    private readonly AppDbContext _context;

    public CriarUsuarioHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CriarUsuarioCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new InvalidOperationException("Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(request.Senha) || request.Senha.Length < 6)
            throw new InvalidOperationException("Senha deve ter pelo menos 6 caracteres.");

        var nomeEmUso = await _context.Usuarios
            .AnyAsync(u => u.Nome == request.Nome, cancellationToken);
        if (nomeEmUso)
            throw new InvalidOperationException($"Já existe um usuário com o nome '{request.Nome}'.");

        var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);
        var usuario = new Usuario(request.Nome, senhaHash, request.Nivel);

        await _context.Usuarios.AddAsync(usuario, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return usuario.Id;
    }
}