using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;
using SgaAutoEletrica.Application.Features.OrdensServico.Queries;
using SgaAutoEletrica.Domain.Enums;

namespace SgaAutoEletrica.UI.ViewModels.OrdensServico;

public class ListaOSViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private const int TamanhoPagina = 40;

    public ObservableCollection<OrdemServicoResumoDTO> Ordens { get; } = new();

    private OrdemServicoResumoDTO? _osSelecionada;
    public OrdemServicoResumoDTO? OsSelecionada
    {
        get => _osSelecionada;
        set { _osSelecionada = value; OnPropertyChanged(); }
    }

    private string _termoBusca = string.Empty;
    public string TermoBusca
    {
        get => _termoBusca;
        set { _termoBusca = value; OnPropertyChanged(); }
    }

    private bool _mostrarCanceladas;
    public bool MostrarCanceladas
    {
        get => _mostrarCanceladas;
        set
        {
            _mostrarCanceladas = value;
            OnPropertyChanged();
            _ = BuscarAsync();
        }
    }

    private bool _filtroAberta;
    public bool FiltroAberta
    {
        get => _filtroAberta;
        set { _filtroAberta = value; OnPropertyChanged(); _ = BuscarAsync(); }
    }

    private bool _filtroEmAndamento;
    public bool FiltroEmAndamento
    {
        get => _filtroEmAndamento;
        set { _filtroEmAndamento = value; OnPropertyChanged(); _ = BuscarAsync(); }
    }

    private bool _filtroAguardandoPecas;
    public bool FiltroAguardandoPecas
    {
        get => _filtroAguardandoPecas;
        set { _filtroAguardandoPecas = value; OnPropertyChanged(); _ = BuscarAsync(); }
    }

    private bool _filtroFinalizada;
    public bool FiltroFinalizada
    {
        get => _filtroFinalizada;
        set { _filtroFinalizada = value; OnPropertyChanged(); _ = BuscarAsync(); }
    }

    private int _paginaAtual = 1;
    public int PaginaAtual
    {
        get => _paginaAtual;
        set { _paginaAtual = value; OnPropertyChanged(); OnPropertyChanged(nameof(TextoPaginacao)); }
    }

    private int _totalPaginas = 1;
    public int TotalPaginas
    {
        get => _totalPaginas;
        set { _totalPaginas = value; OnPropertyChanged(); OnPropertyChanged(nameof(TextoPaginacao)); }
    }

    private int _totalItens;
    public int TotalItens
    {
        get => _totalItens;
        set { _totalItens = value; OnPropertyChanged(); OnPropertyChanged(nameof(TextoPaginacao)); }
    }

    public string TextoPaginacao =>
        $"Página {PaginaAtual} de {TotalPaginas}  |  Total: {TotalItens} ordens de serviço";

    public bool TemPaginaAnterior => PaginaAtual > 1;
    public bool TemProximaPagina => PaginaAtual < TotalPaginas;

    public ICommand BuscarCommand { get; }
    public ICommand LimparCommand { get; }
    public ICommand NovaOSCommand { get; }
    public ICommand AtualizarCommand { get; }
    public ICommand VerDetalhesCommand { get; }
    public ICommand PaginaAnteriorCommand { get; }
    public ICommand ProximaPaginaCommand { get; }

    public ListaOSViewModel(IMediator mediator)
    {
        _mediator = mediator;

        BuscarCommand = new RelayCommand(async _ => { PaginaAtual = 1; await BuscarAsync(); });
        LimparCommand = new RelayCommand(async _ => { 
            TermoBusca = ""; 
            FiltroAberta = false;
            FiltroEmAndamento = false;
            FiltroAguardandoPecas = false;
            FiltroFinalizada = false;
            MostrarCanceladas = false; 
            PaginaAtual = 1; 
            await BuscarAsync(); 
            });
        NovaOSCommand = new RelayCommand(_ => NovaOS());
        AtualizarCommand = new RelayCommand(async _ => await BuscarAsync());
        VerDetalhesCommand = new RelayCommand(_ => VerDetalhes(), _ => OsSelecionada != null);
        PaginaAnteriorCommand = new RelayCommand(async _ => await IrParaPaginaAnterior(), _ => TemPaginaAnterior);
        ProximaPaginaCommand = new RelayCommand(async _ => await IrParaProximaPagina(), _ => TemProximaPagina);
    }

    public async Task BuscarAsync()
    {
        try
        {
            Ordens.Clear();
            OsSelecionada = null;

            var statusSelecionados = new List<StatusOS>();
            if(FiltroAberta) statusSelecionados.Add(StatusOS.Aberta);
            if(FiltroEmAndamento) statusSelecionados.Add(StatusOS.EmAndamento);
            if(FiltroAguardandoPecas) statusSelecionados.Add(StatusOS.AguardandoPecas);
            if(FiltroFinalizada) statusSelecionados.Add(StatusOS.Finalizada);

            var resultado = await _mediator.Send(new ListarOSQuery
            {
                TermoBusca = TermoBusca,
                Status = statusSelecionados, 
                MostrarCanceladas = MostrarCanceladas,
                Pagina = PaginaAtual,
                TamanhoPagina = TamanhoPagina
            });

            foreach (var os in resultado.Itens)
                Ordens.Add(os);

            TotalItens = resultado.TotalItens;
            TotalPaginas = resultado.TotalPaginas;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void NovaOS()
    {
        var dialog = new Views.OrdensServico.CriacaoOSWindow(
            App.ServiceProvider.GetRequiredService<IMediator>());
        if (dialog.ShowDialog() == true)
        {
            _ = BuscarAsync();
        }
    }

    public void VerDetalhes()
    {
        if (OsSelecionada == null) return;
        var dialog = new Views.OrdensServico.DetalhesOSWindow(
            App.ServiceProvider.GetRequiredService<IMediator>(),
            OsSelecionada.Id);
        dialog.ShowDialog();
        _ = BuscarAsync();
    }

    private async Task IrParaPaginaAnterior()
    {
        if (!TemPaginaAnterior) return;
        PaginaAtual--;
        await BuscarAsync();
    }

    private async Task IrParaProximaPagina()
    {
        if (!TemProximaPagina) return;
        PaginaAtual++;
        await BuscarAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}