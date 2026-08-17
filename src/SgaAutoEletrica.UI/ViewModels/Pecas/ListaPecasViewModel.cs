using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using SgaAutoEletrica.Application.Features.Pecas.Commands;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;
using SgaAutoEletrica.Application.Features.Pecas.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Pecas;

public class ListaPecasViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;

    public ObservableCollection<PecaDTO> Pecas { get; } = new();

    private PecaDTO? _pecaSelecionada;
    public PecaDTO? PecaSelecionada
    {
        get => _pecaSelecionada;
        set { _pecaSelecionada = value; OnPropertyChanged(); }
    }

    private string _termoBusca = string.Empty;
    public string TermoBusca
    {
        get => _termoBusca;
        set { _termoBusca = value; OnPropertyChanged(); }
    }

    public ICommand BuscarCommand { get; }
    public ICommand LimparCommand { get; }
    public ICommand NovaPecaCommand { get; }
    public ICommand EditarPecaCommand { get; }
    public ICommand ExcluirPecaCommand { get; }
    public ICommand MovimentarEstoqueCommand { get; }

    public ListaPecasViewModel(IMediator mediator)
    {
        _mediator = mediator;

        BuscarCommand = new RelayCommand(async _ => await BuscarAsync());
        LimparCommand = new RelayCommand(async _ => { TermoBusca = string.Empty; await BuscarAsync(); });
        NovaPecaCommand = new RelayCommand(async _ => await NovaPecaAsync());
        EditarPecaCommand = new RelayCommand(async _ => await EditarPecaAsync(), _ => PecaSelecionada != null);
        ExcluirPecaCommand = new RelayCommand(async _ => await ExcluirPecaAsync(), _ => PecaSelecionada != null);
        MovimentarEstoqueCommand = new RelayCommand(async _ => await MovimentarEstoqueAsync(), _ => PecaSelecionada != null);
    }

    public async Task BuscarAsync()
    {
        try
        {
            Pecas.Clear();
            var resultado = await _mediator.Send(new ListarPecasQuery { TermoBusca = TermoBusca });
            foreach (var peca in resultado)
                Pecas.Add(peca);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro na busca: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task NovaPecaAsync()
    {
        var dialog = new Views.Pecas.CadastroPecaWindow(_mediator);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    private async Task EditarPecaAsync()
    {
        if (PecaSelecionada == null) return;
        var dialog = new Views.Pecas.CadastroPecaWindow(_mediator, PecaSelecionada.Id);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    private async Task ExcluirPecaAsync()
    {
        if (PecaSelecionada == null) return;

        var confirmacao = MessageBox.Show(
            $"Deseja realmente excluir a peça '{PecaSelecionada.Nome}'?",
            "Confirmar exclusão",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirmacao == MessageBoxResult.Yes)
        {
            await _mediator.Send(new ExcluirPecaCommand { Id = PecaSelecionada.Id });
            await BuscarAsync();
        }
    }

    private async Task MovimentarEstoqueAsync()
    {
        if (PecaSelecionada == null) return;

        var dialog = new Views.Pecas.MovimentacaoEstoqueWindow(_mediator, PecaSelecionada);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}