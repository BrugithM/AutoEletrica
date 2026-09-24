using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Features.Buscas.DTOs;
using SgaAutoEletrica.Application.Features.Buscas.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Buscas;

public class BuscarNotasEmitidasViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private const int TamanhoPagina = 20;

    public ObservableCollection<NotaFiscalSaidaResumoDTO> Notas { get; } = new();

    private NotaFiscalSaidaResumoDTO? _notaSelecionada;
    public NotaFiscalSaidaResumoDTO? NotaSelecionada
    {
        get => _notaSelecionada;
        set
        {
            _notaSelecionada = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TemNotaSelecionada));
        }
    }

    public bool TemNotaSelecionada => NotaSelecionada != null;

    // Filtros
    public string Placa { get; set; } = string.Empty;
    public string NomeCliente { get; set; } = string.Empty;
    public string NomePeca { get; set; } = string.Empty;
    public string Observacao { get; set; } = string.Empty;
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
    public ICommand ImprimirNFCommand { get; }
    public ICommand VerDetalhesCommand { get; }
    public ICommand PaginaAnteriorCommand { get; }
    public ICommand ProximaPaginaCommand { get; }

    public BuscarNotasEmitidasViewModel(IMediator mediator)
    {
        _mediator = mediator;

        BuscarCommand = new RelayCommand(async _ => { PaginaAtual = 1; await BuscarAsync(); });
        LimparCommand = new RelayCommand(async _ => { LimparFiltros(); PaginaAtual = 1; await BuscarAsync(); });
        ImprimirNFCommand = new RelayCommand(_ => ImprimirNF(), _ => TemNotaSelecionada);
        VerDetalhesCommand = new RelayCommand(_ => VerDetalhes(), _ => TemNotaSelecionada);
        PaginaAnteriorCommand = new RelayCommand(async _ => await IrParaPaginaAnterior(), _ => TemPaginaAnterior);
        ProximaPaginaCommand = new RelayCommand(async _ => await IrParaProximaPagina(), _ => TemProximaPagina);
    }

    public async Task BuscarAsync()
    {
        try
        {
            Notas.Clear();
            NotaSelecionada = null;

            var resultado = await _mediator.Send(new BuscarNotasEmitidasQuery
            {
                Placa = string.IsNullOrWhiteSpace(Placa) ? null : Placa,
                NomeCliente = string.IsNullOrWhiteSpace(NomeCliente) ? null : NomeCliente,
                NomePeca = string.IsNullOrWhiteSpace(NomePeca) ? null : NomePeca,
                Observacao = string.IsNullOrWhiteSpace(Observacao) ? null : Observacao,
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

    public void VerDetalhes()
    {
        if (NotaSelecionada == null) return;

        var dialog = new Views.Buscas.DetalhesNotaFiscalSaidaWindow(
            App.ServiceProvider.GetRequiredService<IMediator>(),
            NotaSelecionada.Id);
        dialog.ShowDialog();
    }

    public void ImprimirNF()
    {
        if (NotaSelecionada == null) return;

        try
        {
            var impressao = App.ServiceProvider.GetRequiredService<SgaAutoEletrica.Application.Common.Interfaces.IImpressaoService>();
            var nf = _mediator.Send(new ObterNotaFiscalSaidaPorIdQuery { Id = NotaSelecionada.Id })
                .GetAwaiter().GetResult();

            if (nf != null)
            {
                var osDto = new SgaAutoEletrica.Application.Features.OrdensServico.DTOs.OrdemServicoDetalheDTO
                {
                    Numero = 0,
                    NomeCliente = nf.NomeCliente,
                    TelefoneCliente = nf.TelefoneCliente,
                    PlacaVeiculo = nf.PlacaVeiculo,
                    ModeloVeiculo = nf.ModeloVeiculo,
                    MarcaVeiculo = nf.MarcaVeiculo,
                    ValorTotal = nf.ValorTotal,
                    Observacao = nf.Observacao,
                    ItensPeca = nf.Itens.Select(i => new SgaAutoEletrica.Application.Features.OrdensServico.DTOs.ItemPecaOSDTO
                    {
                        NomePeca = i.Descricao,
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.ValorUnitario,
                        ValorTotal = i.ValorTotal
                    }).ToList()
                };

                impressao.ImprimirNotaFiscal(osDto, nf.Numero);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao imprimir: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LimparFiltros()
    {
        Placa = string.Empty;
        NomeCliente = string.Empty;
        NomePeca = string.Empty;
        Observacao = string.Empty;
        DataInicio = null;
        DataFim = null;

        OnPropertyChanged(nameof(Placa));
        OnPropertyChanged(nameof(NomeCliente));
        OnPropertyChanged(nameof(NomePeca));
        OnPropertyChanged(nameof(Observacao));
        OnPropertyChanged(nameof(DataInicio));
        OnPropertyChanged(nameof(DataFim));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}