using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using SgaAutoEletrica.Application.Features.Pecas.Queries;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;
using SgaAutoEletrica.Application.Features.Servicos.Queries;
using SgaAutoEletrica.Application.Features.Servicos.DTOs;
using SgaAutoEletrica.Application.Features.OrdensServico.Commands;
using SgaAutoEletrica.Application.Features.OrdensServico.Queries;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;

namespace SgaAutoEletrica.UI.ViewModels.OrdensServico;

public class EditarOSViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private readonly Guid _osId;

    public ObservableCollection<PecaDTO> PecasDisponiveis { get; } = new();
    public ObservableCollection<ServicoDTO> ServicosDisponiveis { get; } = new();
    public ObservableCollection<ItemPecaOSDTO> PecasNaOS { get; } = new();
    public ObservableCollection<ItemServicoOSDTO> ServicosNaOS { get; } = new();

    private PecaDTO? _pecaSelecionada;
    public PecaDTO? PecaSelecionada
    {
        get => _pecaSelecionada;
        set { _pecaSelecionada = value; OnPropertyChanged(); }
    }

    private ServicoDTO? _servicoSelecionado;
    public ServicoDTO? ServicoSelecionado
    {
        get => _servicoSelecionado;
        set { _servicoSelecionado = value; OnPropertyChanged(); }
    }

    private ItemPecaOSDTO? _pecaSelecionadaParaRemover;
    public ItemPecaOSDTO? PecaSelecionadaParaRemover
    {
        get => _pecaSelecionadaParaRemover;
        set { _pecaSelecionadaParaRemover = value; OnPropertyChanged(); }
    }

    private ItemServicoOSDTO? _servicoSelecionadoParaRemover;
    public ItemServicoOSDTO? ServicoSelecionadoParaRemover
    {
        get => _servicoSelecionadoParaRemover;
        set { _servicoSelecionadoParaRemover = value; OnPropertyChanged(); }
    }

    private int _quantidadePeca = 1;
    public int QuantidadePeca
    {
        get => _quantidadePeca;
        set { _quantidadePeca = value; OnPropertyChanged(); }
    }

    public string Observacao { get; set; } = string.Empty;
    public string Titulo { get; private set; } = "Editar OS";
    public string InfoCliente { get; private set; } = string.Empty;
    public string InfoVeiculo { get; private set; } = string.Empty;

    public decimal ValorTotalPecas => PecasNaOS.Sum(p => p.ValorTotal);
    public decimal ValorTotalServicos => ServicosNaOS.Sum(s => s.PrecoUnitario);
    public decimal ValorTotalGeral => ValorTotalPecas + ValorTotalServicos;

    public ICommand AdicionarPecaCommand { get; }
    public ICommand AdicionarServicoCommand { get; }
    public ICommand RemoverPecaCommand { get; }
    public ICommand RemoverServicoCommand { get; }
    public ICommand SalvarCommand { get; }

    public EditarOSViewModel(IMediator mediator, Guid osId)
    {
        _mediator = mediator;
        _osId = osId;

        AdicionarPecaCommand = new RelayCommand(async _ => await AdicionarPecaAsync());
        AdicionarServicoCommand = new RelayCommand(async _ => await AdicionarServicoAsync());
        RemoverPecaCommand = new RelayCommand(async _ => await RemoverPecaAsync(), _ => PecaSelecionadaParaRemover != null);
        RemoverServicoCommand = new RelayCommand(async _ => await RemoverServicoAsync(), _ => ServicoSelecionadoParaRemover != null);
        SalvarCommand = new RelayCommand(async _ => await SalvarAsync());
    }

    public async Task CarregarDadosAsync()
    {
        var os = await _mediator.Send(new ObterOSPorIdQuery { Id = _osId })
            ?? throw new InvalidOperationException("OS não encontrada.");

        Titulo = $"Editar OS Nº {os.Numero}";
        InfoCliente = $"Cliente: {os.NomeCliente}";
        InfoVeiculo = $"Veículo: {os.MarcaVeiculo} {os.ModeloVeiculo} - {os.PlacaVeiculo}";
        Observacao = os.Observacao ?? "";

        OnPropertyChanged(nameof(Titulo));
        OnPropertyChanged(nameof(InfoCliente));
        OnPropertyChanged(nameof(InfoVeiculo));
        OnPropertyChanged(nameof(Observacao));

        foreach (var peca in os.ItensPeca)
            PecasNaOS.Add(peca);

        foreach (var servico in os.ItensServico)
            ServicosNaOS.Add(servico);

        var pecas = await _mediator.Send(new ListarPecasQuery { Ativo = true });
        foreach (var p in pecas)
            PecasDisponiveis.Add(p);

        var servicos = await _mediator.Send(new ListarServicosQuery());
        foreach (var s in servicos)
            ServicosDisponiveis.Add(s);
    }

    private async Task AdicionarPecaAsync()
    {
        if (PecaSelecionada == null || QuantidadePeca <= 0) return;

        try
        {
            await _mediator.Send(new AdicionarPecaOSCommand
            {
                OrdemServicoId = _osId,
                PecaId = PecaSelecionada.Id,
                Quantidade = QuantidadePeca
            });

            var os = await _mediator.Send(new ObterOSPorIdQuery { Id = _osId });
            PecasNaOS.Clear();
            foreach (var peca in os!.ItensPeca)
                PecasNaOS.Add(peca);

            OnPropertyChanged(nameof(ValorTotalPecas));
            OnPropertyChanged(nameof(ValorTotalGeral));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task AdicionarServicoAsync()
    {
        if (ServicoSelecionado == null) return;

        try
        {
            await _mediator.Send(new AdicionarServicoOSCommand
            {
                OrdemServicoId = _osId,
                ServicoId = ServicoSelecionado.Id
            });

            var os = await _mediator.Send(new ObterOSPorIdQuery { Id = _osId });
            ServicosNaOS.Clear();
            foreach (var servico in os!.ItensServico)
                ServicosNaOS.Add(servico);

            OnPropertyChanged(nameof(ValorTotalServicos));
            OnPropertyChanged(nameof(ValorTotalGeral));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task RemoverPecaAsync()
{
    if (PecaSelecionadaParaRemover == null) return;

    try
    {
        await _mediator.Send(new RemoverItemPecaOSCommand
        {
            OrdemServicoId = _osId,
            ItemId = PecaSelecionadaParaRemover.Id
        });

        var os = await _mediator.Send(new ObterOSPorIdQuery { Id = _osId });
        PecasNaOS.Clear();
        foreach (var peca in os!.ItensPeca)
            PecasNaOS.Add(peca);

        OnPropertyChanged(nameof(ValorTotalPecas));
        OnPropertyChanged(nameof(ValorTotalGeral));
    }
    catch (Exception ex)
    {
        var inner = ex;
        while (inner.InnerException != null)
            inner = inner.InnerException;
        MessageBox.Show($"Erro ao remover peça: {inner.Message}\n\n{inner.StackTrace}", 
            "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

   private async Task RemoverServicoAsync()
{
    if (ServicoSelecionadoParaRemover == null) return;

    try
    {
        await _mediator.Send(new RemoverItemServicoOSCommand
        {
            OrdemServicoId = _osId,
            ItemId = ServicoSelecionadoParaRemover.Id
        });

        var os = await _mediator.Send(new ObterOSPorIdQuery { Id = _osId });
        ServicosNaOS.Clear();
        foreach (var servico in os!.ItensServico)
            ServicosNaOS.Add(servico);

        OnPropertyChanged(nameof(ValorTotalServicos));
        OnPropertyChanged(nameof(ValorTotalGeral));
    }
    catch (Exception ex)
    {
        var inner = ex;
        while (inner.InnerException != null)
            inner = inner.InnerException;
        MessageBox.Show($"Erro ao remover serviço: {inner.Message}\n\n{inner.StackTrace}", 
            "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

    private async Task SalvarAsync()
{
    try
    {
        await _mediator.Send(new AtualizarObservacaoOSCommand
        {
            OrdemServicoId = _osId,
            Observacao = Observacao
        });

        MessageBox.Show("OS atualizada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}