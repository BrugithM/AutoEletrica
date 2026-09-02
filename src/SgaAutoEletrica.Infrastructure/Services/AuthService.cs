using Microsoft.EntityFrameworkCore;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Domain.Entities;
using SgaAutoEletrica.Domain.Enums;
using SgaAutoEletrica.Infrastructure.Persistence.Context;

namespace SgaAutoEletrica.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> Autenticar(string nome, string senha, CancellationToken cancellationToken = default)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Nome == nome && u.Ativo, cancellationToken);

        if (usuario == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash))
            return null;

        usuario.AtualizarUltimoLogin();
        await _context.SaveChangesAsync(cancellationToken);

        return usuario;
    }

    public async Task<List<Usuario>> ListarUsuarios(CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .OrderBy(u => u.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task CriarUsuario(string nome, string senha, NivelUsuario nivel, CancellationToken cancellationToken = default)
    {
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);
        var usuario = new Usuario(nome, senhaHash, nivel);
        await _context.Usuarios.AddAsync(usuario, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AlterarSenha(Guid usuarioId, string novaSenha, CancellationToken cancellationToken = default)
    {
        var usuario = await _context.Usuarios.FindAsync([usuarioId], cancellationToken)
            ?? throw new InvalidOperationException("Usuário não encontrado.");

        var novaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha);
        usuario.AlterarSenha(novaHash);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AlterarNivel(Guid usuarioId, NivelUsuario novoNivel, CancellationToken cancellationToken = default)
    {
        var usuario = await _context.Usuarios.FindAsync([usuarioId], cancellationToken)
            ?? throw new InvalidOperationException("Usuário não encontrado.");

        usuario.AlterarNivel(novoNivel);
        await _context.SaveChangesAsync(cancellationToken);
    }
}