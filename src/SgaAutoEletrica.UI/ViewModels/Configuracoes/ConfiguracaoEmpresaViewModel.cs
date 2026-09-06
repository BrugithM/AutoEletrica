using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using SgaAutoEletrica.Application.Features.Configuracoes.Commands;
using SgaAutoEletrica.Application.Features.Configuracoes.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Configuracoes;

public class ConfiguracaoEmpresaViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public string NomeEmpresa { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public ICommand SalvarCommand { get; }

    public ConfiguracaoEmpresaViewModel(IMediator mediator)
    {
        _mediator = mediator;
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

            OnPropertyChanged(nameof(NomeEmpresa));
            OnPropertyChanged(nameof(Cnpj));
            OnPropertyChanged(nameof(Telefone));
            OnPropertyChanged(nameof(Endereco));
            OnPropertyChanged(nameof(Email));
        }
    }

    private async Task SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(NomeEmpresa))
        {
            MessageBox.Show("Nome da empresa é obrigatório.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(Cnpj))
        {
            MessageBox.Show("CNPJ é obrigatório.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(Telefone))
        {
            MessageBox.Show("Telefone é obrigatório.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        await _mediator.Send(new SalvarConfiguracaoEmpresaCommand
        {
            NomeEmpresa = NomeEmpresa,
            Cnpj = Cnpj,
            Telefone = Telefone,
            Endereco = Endereco,
            Email = Email
        });

        MessageBox.Show("Configurações salvas com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}