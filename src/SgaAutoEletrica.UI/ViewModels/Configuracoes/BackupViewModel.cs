using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.IO;
using SgaAutoEletrica.Application.Common.Interfaces;

namespace SgaAutoEletrica.UI.ViewModels.Configuracoes;

public class BackupViewModel : INotifyPropertyChanged
{
    private readonly IBackupService _backupService;

    public ObservableCollection<string> Backups { get; } = new();

    public ICommand RealizarBackupCommand { get; }
    public ICommand RestaurarCommand { get; }
    public ICommand AtualizarCommand { get; }

    public BackupViewModel(IBackupService backupService)
    {
        _backupService = backupService;
        RealizarBackupCommand = new RelayCommand(async _ => await RealizarBackupAsync());
        RestaurarCommand = new RelayCommand(async _ => await RestaurarAsync());
        AtualizarCommand = new RelayCommand(async _ => await AtualizarListaAsync());
    }

    public async Task AtualizarListaAsync()
    {
        Backups.Clear();
        var lista = await _backupService.ListarBackups();
        foreach (var backup in lista)
            Backups.Add(Path.GetFileName(backup));
    }

    private async Task RealizarBackupAsync()
    {
        var caminho = await _backupService.RealizarBackupManual();
        MessageBox.Show($"Backup criado com sucesso!\n\n{caminho}", "Backup", MessageBoxButton.OK, MessageBoxImage.Information);
        await AtualizarListaAsync();
    }

    private async Task RestaurarAsync()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Arquivos de banco (*.db)|*.db|Todos os arquivos (*.*)|*.*",
            Title = "Selecionar arquivo de backup"
        };

        if (dialog.ShowDialog() == true)
        {
            var confirmacao = MessageBox.Show(
                "Restaurar backup substituirá os dados atuais. Deseja continuar?",
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmacao == MessageBoxResult.Yes)
            {
                await _backupService.RestaurarBackup(dialog.FileName);
                MessageBox.Show("Backup restaurado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}