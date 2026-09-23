using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Features.Buscas.DTOs;
using SgaAutoEletrica.Application.Features.Buscas.Queries;
using SgaAutoEletrica.Application.Features.Fornecedores.DTOs;
using SgaAutoEletrica.Application.Features.Fornecedores.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Buscas;

public class BuscarNotasEntradaViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private const int TamanhoPagina = 20;

    public ObservableCollection<NotaFiscalEntradaResumoDTO> Notas { get; } = new();
    public ObservableCollection<ItemNotaEntradaDetalheDTO> ItensNF { get; } = new();

    private NotaFiscalEntradaResumoDTO? _notaSelecionada;
    public NotaFiscalEntradaResumoDTO? NotaSelecionada
    {
        get => _notaSelecionada;
        set
        {
            _notaSelecionada = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TemNotaSelecionada));
            OnPropertyChanged(nameof(TituloItens));
            _ = CarregarItensAsync();
        }
    }

    public bool TemNotaSelecionada => NotaSelecionada != null;
    public string TituloItens => NotaSelecionada != null
        ? $"Itens da Nota Fiscal Nº {NotaSelecionada.Numero}:"
        : "";

    // Filtros
    public string NomeFornecedor { get; set; } = string.Empty;
    public string CnpjFornecedor { get; set; } = string.Empty;
    public string CodigoProduto { get; set; } = string.Empty;
    public string NomeProduto { get; set; } = string.Empty;
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }

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
        $"Página {PaginaAtual} de {TotalPaginas}  |  Total: {TotalItens} notas fiscais";

    public bool TemPaginaAnterior => PaginaAtual > 1;
    public bool TemProximaPagina => PaginaAtual < TotalPaginas;

    public ICommand BuscarCommand { get; }
    public ICommand LimparCommand { get; }
    public ICommand NovaNFCommand { get; }
    public ICommand PaginaAnteriorCommand { get; }
    public ICommand ProximaPaginaCommand { get; }

    public BuscarNotasEntradaViewModel(IMediator mediator)
    {
        _mediator = mediator;

        BuscarCommand = new RelayCommand(async _ => { PaginaAtual = 1; await BuscarAsync(); });
        LimparCommand = new RelayCommand(async _ => { LimparFiltros(); PaginaAtual = 1; await BuscarAsync(); });
        NovaNFCommand = new RelayCommand(_ => NovaNF());
        PaginaAnteriorCommand = new RelayCommand(async _ => await IrParaPaginaAnterior(), _ => TemPaginaAnterior);
        ProximaPaginaCommand = new RelayCommand(async _ => await IrParaProximaPagina(), _ => TemProximaPagina);
    }

    public async Task BuscarAsync()
    {
        try
        {
            Notas.Clear();
            ItensNF.Clear();
            NotaSelecionada = null;

            var resultado = await _mediator.Send(new BuscarNotasEntradaQuery
            {
                NomeFornecedor = string.IsNullOrWhiteSpace(NomeFornecedor) ? null : NomeFornecedor,
                CnpjFornecedor = string.IsNullOrWhiteSpace(CnpjFornecedor) ? null : CnpjFornecedor,
                CodigoProduto = string.IsNullOrWhiteSpace(CodigoProduto) ? null : CodigoProduto,
                NomeProduto = string.IsNullOrWhiteSpace(NomeProduto) ? null : NomeProduto,
                DataInicio = DataInicio,
                DataFim = DataFim,
                Pagina = PaginaAtual,
                TamanhoPagina = TamanhoPagina
            });

            foreach (var nf in resultado.Itens)
                Notas.Add(nf);

            TotalItens = resultado.TotalItens;
            TotalPaginas = resultado.TotalPaginas;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task CarregarItensAsync()
    {
        ItensNF.Clear();
        if (NotaSelecionada == null) return;

        var nf = await _mediator.Send(new ObterNotaFiscalEntradaPorIdQuery { Id = NotaSelecionada.Id });
        if (nf != null)
        {
            foreach (var item in nf.Itens)
                ItensNF.Add(item);
        }
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

    private void NovaNF()
    {
        var dialog = new Views.Fornecedores.CriarNotaFiscalEntradaWindow(
            App.ServiceProvider.GetRequiredService<IMediator>());
        if (dialog.ShowDialog() == true)
        {
            _ = BuscarAsync();
        }
    }

    private void LimparFiltros()
    {
        NomeFornecedor = string.Empty;
        CnpjFornecedor = string.Empty;
        CodigoProduto = string.Empty;
        NomeProduto = string.Empty;
        DataInicio = null;
        DataFim = null;

        OnPropertyChanged(nameof(NomeFornecedor));
        OnPropertyChanged(nameof(CnpjFornecedor));
        OnPropertyChanged(nameof(CodigoProduto));
        OnPropertyChanged(nameof(NomeProduto));
        OnPropertyChanged(nameof(DataInicio));
        OnPropertyChanged(nameof(DataFim));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}