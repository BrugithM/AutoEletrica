using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MediatR;
using SgaAutoEletrica.Application.Features.Fornecedores.Commands;
using SgaAutoEletrica.Application.Features.Fornecedores.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Fornecedores;

public class CadastroFornecedorViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private readonly Guid? _fornecedorId;

    public string NomeEmpresa { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Contato { get; set; } = string.Empty;

    private string _titulo = "Novo Fornecedor";
    public string Titulo
    {
        get => _titulo;
        private set { _titulo = value; OnPropertyChanged(); }
    }

    public CadastroFornecedorViewModel(IMediator mediator, Guid? fornecedorId = null)
    {
        _mediator = mediator;
        _fornecedorId = fornecedorId;

        if (fornecedorId.HasValue)
        {
            Titulo = "Editar Fornecedor";
            CarregarDadosAsync(fornecedorId.Value);
        }
    }

    private async void CarregarDadosAsync(Guid fornecedorId)
    {
        var fornecedor = await _mediator.Send(new ObterFornecedorPorIdQuery { Id = fornecedorId });
        if (fornecedor != null)
        {
            NomeEmpresa = fornecedor.NomeEmpresa;
            Cnpj = fornecedor.Cnpj;
            Telefone = fornecedor.Telefone ?? "";
            Contato = fornecedor.Contato ?? "";
            OnPropertyChanged(nameof(NomeEmpresa));
            OnPropertyChanged(nameof(Cnpj));
            OnPropertyChanged(nameof(Telefone));
            OnPropertyChanged(nameof(Contato));
        }
    }

    public async Task<bool> SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(NomeEmpresa))
        {
            MessageBox.Show("Nome da empresa é obrigatório.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        if (string.IsNullOrWhiteSpace(Cnpj))
        {
            MessageBox.Show("CNPJ é obrigatório.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (_fornecedorId.HasValue)
        {
            await _mediator.Send(new AtualizarFornecedorCommand
            {
                Id = _fornecedorId.Value,
                NomeEmpresa = NomeEmpresa,
                Telefone = Telefone,
                Contato = Contato
            });
        }
        else
        {
            await _mediator.Send(new CriarFornecedorCommand
            {
                NomeEmpresa = NomeEmpresa,
                Cnpj = Cnpj,
                Telefone = Telefone,
                Contato = Contato
            });
        }

        return true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}