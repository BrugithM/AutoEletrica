using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Application.Features.Clientes.Commands;
using SgaAutoEletrica.Application.Features.Clientes.DTOs;
using SgaAutoEletrica.Application.Features.Clientes.Queries;
using SgaAutoEletrica.UI.Views.Clientes;

namespace SgaAutoEletrica.UI.ViewModels.Clientes;

public class ListaClientesViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    public ObservableCollection<ClienteDTO> Clientes { get; } = new();
    private ClienteDTO? _clienteSelecionado;
    public bool EhAdministrador => App.ServiceProvider
        .GetRequiredService<ISessaoUsuario>().EhAdministrador;

    public ClienteDTO? ClienteSelecionado
    {
        get => _clienteSelecionado;
        set { _clienteSelecionado = value; OnPropertyChanged(); }
    }

    private string _termoBusca = string.Empty;
    public string TermoBusca
    {
        get => _termoBusca;
        set { _termoBusca = value; OnPropertyChanged(); }
    }

    public ICommand BuscarCommand { get; }
    public ICommand LimparBuscaCommand { get; }
    public ICommand NovoClienteCommand { get; }
    public ICommand EditarClienteCommand { get; }
    public ICommand ExcluirClienteCommand { get; }

    public ListaClientesViewModel(IMediator mediator)
    {
        _mediator = mediator;
        
        BuscarCommand = new RelayCommand(async _ => await BuscarAsync());
        LimparBuscaCommand = new RelayCommand(async _ => { TermoBusca = ""; await BuscarAsync(); });
        NovoClienteCommand = new RelayCommand(async _ => await NovoClienteAsync());
        EditarClienteCommand = new RelayCommand(async _ => await EditarClienteAsync(), _ => ClienteSelecionado != null);
        ExcluirClienteCommand = new RelayCommand(async _ => await ExcluirClienteAsync(), _ => ClienteSelecionado != null && EhAdministrador);
    }

    public async Task BuscarAsync()
    {
        try
        {
            Clientes.Clear();
            var resultado = await _mediator.Send(new ListarClientesQuery { TermoBusca = TermoBusca });
            foreach (var cliente in resultado)
                Clientes.Add(cliente);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task NovoClienteAsync()
    {
        var dialog = new CadastroClienteWindow(_mediator);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    private async Task EditarClienteAsync()
    {
        if (ClienteSelecionado == null) return;
        var dialog = new CadastroClienteWindow(_mediator, ClienteSelecionado.Id);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    private async Task ExcluirClienteAsync()
    {
        if (ClienteSelecionado == null) return;
        var confirmacao = MessageBox.Show(
            $"Realmente deseja excluir o cliente '{ClienteSelecionado.NomeCompleto}'?",
            "Confirmar exclusão",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirmacao == MessageBoxResult.Yes)
        {
            await _mediator.Send(new ExcluirClienteCommand { Id = ClienteSelecionado.Id });
            await BuscarAsync();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}