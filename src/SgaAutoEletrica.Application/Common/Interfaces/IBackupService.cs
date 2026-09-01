namespace SgaAutoEletrica.Application.Common.Interfaces;

public interface IBackupService
{
    Task<string> RealizarBackupManual(string? caminhoDestino = null);
    Task<string> RealizarBackupAutomatico();
    Task RestaurarBackup(string caminhoArquivo);
    Task<List<string>> ListarBackups();
}