using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using SgaAutoEletrica.Application.Features.Veiculos.Commands;
using SgaAutoEletrica.Application.Features.Veiculos.DTOs;
using SgaAutoEletrica.Application.Features.Veiculos.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Veiculos;

public class ListaVeiculosViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public ObservableCollection<VeiculoDTO> Veiculos { get; } = new();

    private VeiculoDTO? _veiculoSelecionado;
    public VeiculoDTO? VeiculoSelecionado
    {
        get => _veiculoSelecionado;
        set { _veiculoSelecionado = value; OnPropertyChanged(); }
    }

    private string _termoBusca = string.Empty;
    public string TermoBusca
    {
        get => _termoBusca;
        set { _termoBusca = value; OnPropertyChanged(); }
    }

    public ICommand BuscarCommand { get; }
    public ICommand LimparCommand { get; }
    public ICommand NovoVeiculoCommand { get; }
    public ICommand EditarVeiculoCommand { get; }
    public ICommand ExcluirVeiculoCommand { get; }

    public ListaVeiculosViewModel(IMediator mediator)
    {
        _mediator = mediator;

        BuscarCommand = new RelayCommand(async _ => await BuscarAsync());
        LimparCommand = new RelayCommand(async _ => 
        { 
            TermoBusca = string.Empty; 
            await BuscarAsync(); 
        });
        NovoVeiculoCommand = new RelayCommand(async _ => await NovoVeiculoAsync());
        EditarVeiculoCommand = new RelayCommand(async _ => await EditarVeiculoAsync(), _ => VeiculoSelecionado != null);
        ExcluirVeiculoCommand = new RelayCommand(async _ => await ExcluirVeiculoAsync(), _ => VeiculoSelecionado != null);
    }

    public async Task BuscarAsync()
    {
        try
        {
            Veiculos.Clear();
            var resultado = await _mediator.Send(new BuscarVeiculosQuery { TermoBusca = TermoBusca });
            foreach (var veiculo in resultado)
                Veiculos.Add(veiculo);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro na busca: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task NovoVeiculoAsync()
    {
        var dialog = new Views.Veiculos.CadastroVeiculoWindow(_mediator);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    private async Task EditarVeiculoAsync()
    {
        if (VeiculoSelecionado == null) return;

        var dialog = new Views.Veiculos.CadastroVeiculoWindow(_mediator, VeiculoSelecionado.Id);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    private async Task ExcluirVeiculoAsync()
    {
        if (VeiculoSelecionado == null) return;

        var confirmacao = MessageBox.Show(
            $"Deseja realmente excluir o veículo '{VeiculoSelecionado.Modelo} - {VeiculoSelecionado.Placa}'?",
            "Confirmar exclusão",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirmacao == MessageBoxResult.Yes)
        {
            try
            {
                await _mediator.Send(new ExcluirVeiculoCommand { Id = VeiculoSelecionado.Id });
                await BuscarAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}