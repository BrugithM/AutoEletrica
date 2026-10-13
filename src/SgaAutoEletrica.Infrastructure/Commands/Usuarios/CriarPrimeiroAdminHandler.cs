using MediatR;
using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Features.Usuarios.Commands;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Domain.Enums;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Commands.Usuarios;

public class CriarPrimeiroAdminHandler : IRequestHandler<CriarPrimeiroAdminCommand, Usuario>
{
    private readonly AppDbContext _context;

    public CriarPrimeiroAdminHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario> Handle(CriarPrimeiroAdminCommand request, CancellationToken cancellationToken)
    {
        if (await _context.Usuarios.AnyAsync(cancellationToken))
            throw new InvalidOperationException("Já existe um usuário cadastrado no sistema.");

        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new InvalidOperationException("Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(request.Senha) || request.Senha.Length < 6)
            throw new InvalidOperationException("Senha deve ter pelo menos 6 caracteres.");

        var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);
        var admin = new Usuario(request.Nome, senhaHash, NivelUsuario.Administrador);

        await _context.Usuarios.AddAsync(admin, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return admin;
    }
}