using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Application.Features.Veiculos.Commands;
using SgaAutoEletrica.Application.Features.Veiculos.DTOs;
using SgaAutoEletrica.Application.Features.Veiculos.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Veiculos;

public class ListaVeiculosViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public ObservableCollection<VeiculoDTO> Veiculos { get; } = new();

    public bool EhAdministrador => App.ServiceProvider
        .GetRequiredService<ISessaoUsuario>().EhAdministrador;

    private VeiculoDTO? _veiculoSelecionado;
    public VeiculoDTO? VeiculoSelecionado
    {
        get => _veiculoSelecionado;
        set
        {
            _veiculoSelecionado = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TemVeiculoSelecionado));
            OnPropertyChanged(nameof(ClienteNome));
            OnPropertyChanged(nameof(ClienteCpf));
            OnPropertyChanged(nameof(ClienteTelefone));
            OnPropertyChanged(nameof(TextoBotaoDesativar));
        }
    }

    public bool TemVeiculoSelecionado => VeiculoSelecionado != null;
    public string ClienteNome => VeiculoSelecionado?.NomeCliente ?? "";
    public string ClienteCpf => VeiculoSelecionado?.CpfCliente ?? "";
    public string ClienteTelefone => VeiculoSelecionado?.TelefoneCliente ?? "";

    public string TextoBotaoDesativar =>
        VeiculoSelecionado?.Ativo == false ? "▶️ Reativar" : "⏸️ Desativar";

    private string _termoBusca = string.Empty;
    public string TermoBusca
    {
        get => _termoBusca;
        set { _termoBusca = value; OnPropertyChanged(); }
    }

    private bool _mostrarInativos;
    public bool MostrarInativos
    {
        get => _mostrarInativos;
        set
        {
            _mostrarInativos = value;
            OnPropertyChanged();
            _ = BuscarAsync();
        }
    }

    public ICommand BuscarCommand { get; }
    public ICommand LimparCommand { get; }
    public ICommand NovoVeiculoCommand { get; }
    public ICommand EditarVeiculoCommand { get; }
    public ICommand DesativarVeiculoCommand { get; }
    public ICommand AtualizarCommand { get; }
    public ICommand NFsVinculadasCommand { get; }
    public ICommand CriarOSCommand { get; }

    public ListaVeiculosViewModel(IMediator mediator)
    {
        _mediator = mediator;

        BuscarCommand = new RelayCommand(async _ => await BuscarAsync());
        LimparCommand = new RelayCommand(async _ => { TermoBusca = ""; await BuscarAsync(); });
        NovoVeiculoCommand = new RelayCommand(async _ => await NovoVeiculoAsync());
        EditarVeiculoCommand = new RelayCommand(async _ => await EditarVeiculoAsync(), _ => TemVeiculoSelecionado);
        DesativarVeiculoCommand = new RelayCommand(async _ => await DesativarVeiculoAsync(), _ => TemVeiculoSelecionado && EhAdministrador);
        AtualizarCommand = new RelayCommand(async _ => await BuscarAsync());
        NFsVinculadasCommand = new RelayCommand(async _ => await NFsVinculadasAsync(), _ => TemVeiculoSelecionado);
        CriarOSCommand = new RelayCommand(async _ => await CriarOSAsync(), _ => TemVeiculoSelecionado);
    }

    public async Task BuscarAsync()
    {
        try
        {
            Veiculos.Clear();
            VeiculoSelecionado = null;
            var resultado = await _mediator.Send(new BuscarVeiculosQuery 
            { 
                TermoBusca = TermoBusca,
                Ativo = MostrarInativos ? null : true
            });
            foreach (var veiculo in resultado)
                Veiculos.Add(veiculo);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task NovoVeiculoAsync()
    {
        var dialog = new Views.Veiculos.CadastroVeiculoWindow(_mediator, null, VeiculoSelecionado?.ClienteId);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    private async Task EditarVeiculoAsync()
    {
        if (VeiculoSelecionado == null) return;
        var dialog = new Views.Veiculos.CadastroVeiculoWindow(_mediator, VeiculoSelecionado.Id, VeiculoSelecionado.ClienteId);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    private async Task DesativarVeiculoAsync()
    {
        if (VeiculoSelecionado == null) return;

        var acao = VeiculoSelecionado.Ativo ? "desativar" : "reativar";
        var confirmacao = MessageBox.Show(
            $"Deseja realmente {acao} o veículo '{VeiculoSelecionado.Modelo} - {VeiculoSelecionado.Placa}'?",
            "Confirmar",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirmacao == MessageBoxResult.Yes)
        {
            if (VeiculoSelecionado.Ativo)
                await _mediator.Send(new DesativarVeiculoCommand { Id = VeiculoSelecionado.Id });
            else
                await _mediator.Send(new ReativarVeiculoCommand { Id = VeiculoSelecionado.Id });

            await BuscarAsync();
        }
    }

    private async Task NFsVinculadasAsync()
    {
        if (VeiculoSelecionado == null) return;
        MessageBox.Show($"Lista de NFs do veículo {VeiculoSelecionado.Placa} - em breve", "Em breve");
    }

    private async Task CriarOSAsync()
    {
        if (VeiculoSelecionado == null) return;
        var dialog = new Views.OrdensServico.CriacaoOSWindow(
            _mediator, 
            VeiculoSelecionado.ClienteId, 
            VeiculoSelecionado.Id);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}