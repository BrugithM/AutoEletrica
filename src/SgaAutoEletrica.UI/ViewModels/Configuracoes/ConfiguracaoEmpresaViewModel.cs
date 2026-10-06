using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MediatR;
using SgaAutoEletrica.Application.Features.Configuracoes.Commands;
using SgaAutoEletrica.Application.Features.Configuracoes.Queries;
using Microsoft.Win32;

namespace SgaAutoEletrica.UI.ViewModels.Configuracoes;

public class ConfiguracaoEmpresaViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private readonly string _pastaImagens;

    private string _nomeEmpresa = string.Empty;
    public string NomeEmpresa
    {
        get => _nomeEmpresa;
        set { _nomeEmpresa = value; OnPropertyChanged(); }
    }

    private string _cnpj = string.Empty;
    public string Cnpj
    {
        get => _cnpj;
        set { _cnpj = value; OnPropertyChanged(); }
    }

    private string _telefone = string.Empty;
    public string Telefone
    {
        get => _telefone;
        set { _telefone = value; OnPropertyChanged(); }
    }

    private string _endereco = string.Empty;
    public string Endereco
    {
        get => _endereco;
        set { _endereco = value; OnPropertyChanged(); }
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); }
    }

    private string? _logoPath;
    public string? LogoPath
    {
        get => _logoPath;
        set { _logoPath = value; OnPropertyChanged(); OnPropertyChanged(nameof(TemLogo)); OnPropertyChanged(nameof(LogoPreview)); }
    }

    public bool TemLogo => !string.IsNullOrWhiteSpace(LogoPath) && File.Exists(LogoPath);

    public BitmapImage? LogoPreview
    {
        get
        {
            if (!TemLogo) return null;

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(LogoPath!, UriKind.Absolute);
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }
    }

    public ICommand SelecionarImagemCommand { get; }
    public ICommand RemoverImagemCommand { get; }
    public ICommand SalvarCommand { get; }

    public ConfiguracaoEmpresaViewModel(IMediator mediator)
    {
        _mediator = mediator;

        _pastaImagens = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SgaAutoEletrica",
            "Config");

        Directory.CreateDirectory(_pastaImagens);

        SelecionarImagemCommand = new RelayCommand(_ => SelecionarImagem());
        RemoverImagemCommand = new RelayCommand(_ => RemoverImagem(), _ => TemLogo);
        SalvarCommand = new RelayCommand(async _ => await SalvarAsync());
    }

    public async Task CarregarAsync()
    {
        var config = await _mediator.Send(new ObterConfiguracaoEmpresaQuery());
        if (config != null)
        {
            NomeEmpresa = config.NomeEmpresa;
            Cnpj = config.Cnpj;
            Telefone = config.Telefone;
            Endereco = config.Endereco ?? string.Empty;
            Email = config.Email ?? string.Empty;
            LogoPath = config.LogoPath;
        }
    }

    private void SelecionarImagem()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Imagens (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp",
            Title = "Selecione a imagem de fundo"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                // Valida tamanho
                var fileInfo = new FileInfo(dialog.FileName);
                if (fileInfo.Length > 5 * 1024 * 1024)
                {
                    MessageBox.Show("A imagem deve ter no máximo 5MB.", "Aviso", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Copia para a pasta do sistema
                var extensao = Path.GetExtension(dialog.FileName);
                var nomeDestino = $"logo{extensao}";
                var caminhoDestino = Path.Combine(_pastaImagens, nomeDestino);

                // Remove versões anteriores
                foreach (var arquivo in Directory.GetFiles(_pastaImagens, "logo.*"))
                {
                    try { File.Delete(arquivo); } catch { }
                }

                File.Copy(dialog.FileName, caminhoDestino, true);
                LogoPath = caminhoDestino;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao copiar imagem: {ex.Message}", "Erro", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void RemoverImagem()
    {
        try
        {
            if (File.Exists(LogoPath))
                File.Delete(LogoPath);
        }
        catch { }

        LogoPath = null;
    }

    private async Task SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(NomeEmpresa))
        {
            MessageBox.Show("Nome da empresa é obrigatório.", "Aviso", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(Cnpj))
        {
            MessageBox.Show("CNPJ é obrigatório.", "Aviso", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(Telefone))
        {
            MessageBox.Show("Telefone é obrigatório.", "Aviso", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            await _mediator.Send(new SalvarConfiguracaoEmpresaCommand
            {
                NomeEmpresa = NomeEmpresa,
                Cnpj = Cnpj,
                Telefone = Telefone,
                Endereco = Endereco,
                Email = Email,
                LogoPath = LogoPath
            });

            MessageBox.Show("Configurações salvas com sucesso!", "Sucesso", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            var inner = ex;
            while (inner.InnerException != null)
                inner = inner.InnerException;
            MessageBox.Show($"Erro: {inner.Message}", "Erro", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}