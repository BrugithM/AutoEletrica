using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MediatR;
using SgaAutoEletrica.Application.Features.Clientes.Commands;

namespace SgaAutoEletrica.UI.ViewModels.Clientes;

public class CadastroClienteViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private readonly Guid? _clienteId;

    public string NomeCompleto{get; set;}= string.Empty;
    public string Cpf {get;set;}=string.Empty;
    public string Telefone {get;set;}=string.Empty;
    public string Logradouro {get;set;}=string.Empty;
    public string Numero {get;set;}=string.Empty;
    public string Complemento {get;set;}=string.Empty;
    public string Bairro {get;set;}=string.Empty;
    public string Cidade {get;set;}=string.Empty;
    public string Estado {get;set;}=string.Empty;
    public string Cep {get;set;}=string.Empty;

    private string _titulo = "NovoCliente";
    public string Titulo
    {
        get=> _titulo;
        private set {_titulo = value;OnPropertyChanged();}
    }

    public CadastroClienteViewModel(IMediator mediator, Guid? clienteId = null)
    {
        _mediator = mediator;
        _clienteId=clienteId;

   if (clienteId.HasValue)
        {
            Titulo = "Editar Cliente";
            CarregarDadosAsync(clienteId.Value);
        }
    }

    private async void CarregarDadosAsync(Guid clienteId)
    {
        var cliente = await _mediator.Send(new Application.Features.Clientes.Queries.ObterClientePorIdQuery { Id = clienteId });
        if (cliente != null)
        {
            NomeCompleto = cliente.NomeCompleto;
            Telefone = cliente.Telefone;
            OnPropertyChanged(nameof(NomeCompleto));
            OnPropertyChanged(nameof(Telefone));
        }
    }

    public async Task<bool> SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(NomeCompleto) || string.IsNullOrWhiteSpace(Cpf))
        {
            MessageBox.Show("Nome e CPF são obrigatórios.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (_clienteId.HasValue)
        {
            await _mediator.Send(new AtualizarClienteCommand
            {
                Id = _clienteId.Value,
                NomeCompleto = NomeCompleto,
                Telefone = Telefone,
                Logradouro = Logradouro,
                Numero = Numero,
                Complemento = Complemento,
                Bairro = Bairro,
                Cidade = Cidade,
                Estado = Estado,
                Cep = Cep
            });
        }
        else
        {
            await _mediator.Send(new CriarClienteCommand
            {
                NomeCompleto = NomeCompleto,
                Cpf = Cpf,
                Telefone = Telefone,
                Logradouro = Logradouro,
                Numero = Numero,
                Complemento = Complemento,
                Bairro = Bairro,
                Cidade = Cidade,
                Estado = Estado,
                Cep = Cep
            });
        }

        return true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}