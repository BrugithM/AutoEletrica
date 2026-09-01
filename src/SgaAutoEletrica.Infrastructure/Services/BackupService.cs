using SgaAutoEletrica.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SgaAutoEletrica.Infrastructure.Services;

public class BackupService : IBackupService
{
    private readonly string _dbPath;
    private readonly string _backupFolder;

    public BackupService(Persistence.Context.AppDbContext context)
    {
        var connectionString = context.Database.GetConnectionString()
            ?? throw new InvalidOperationException("Connection string não encontrada.");

        _dbPath = connectionString.Replace("Data Source=", "");

        var dataFolder = Path.GetDirectoryName(_dbPath) ?? ".";
        _backupFolder = Path.Combine(dataFolder, "Backups");

        Directory.CreateDirectory(_backupFolder);
    }

    public async Task<string> RealizarBackupManual(string? caminhoDestino = null)
    {
        if (!File.Exists(_dbPath))
            throw new FileNotFoundException($"Banco de dados não encontrado em: {_dbPath}");

        var destino = caminhoDestino ?? _backupFolder;
        Directory.CreateDirectory(destino);

        var nomeArquivo = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.db";
        var caminhoCompleto = Path.Combine(destino, nomeArquivo);

        await Task.Run(() => File.Copy(_dbPath, caminhoCompleto, true));

        return caminhoCompleto;
    }

    public async Task<string> RealizarBackupAutomatico()
    {
        var caminho = await RealizarBackupManual(_backupFolder);

        var backups = await ListarBackups();
        if (backups.Count > 10)
        {
            foreach (var arquivo in backups.Skip(10))
            {
                try { File.Delete(arquivo); } catch { }
            }
        }

        return caminho;
    }

    public async Task RestaurarBackup(string caminhoArquivo)
    {
        if (!File.Exists(caminhoArquivo))
            throw new FileNotFoundException("Arquivo de backup não encontrado.");

        await Task.Run(() => File.Copy(caminhoArquivo, _dbPath, true));
    }

    public Task<List<string>> ListarBackups()
    {
        var backups = Directory.GetFiles(_backupFolder, "backup_*.db")
            .OrderByDescending(f => f)
            .ToList();

        return Task.FromResult(backups);
    }
}