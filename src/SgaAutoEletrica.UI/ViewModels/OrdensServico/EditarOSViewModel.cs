using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using SgaAutoEletrica.Application.Features.OrdensServico.Commands;
using SgaAutoEletrica.Application.Features.OrdensServico.DTOs;
using SgaAutoEletrica.Application.Features.OrdensServico.Queries;
using SgaAutoEletrica.Application.Features.Pecas.Queries;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;
using SgaAutoEletrica.Application.Features.Servicos.Queries;
using SgaAutoEletrica.Application.Features.Servicos.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace SgaAutoEletrica.UI.ViewModels.OrdensServico;

public class EditarOSViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private readonly Guid _osId;

    public ObservableCollection<PecaDTO> PecasDisponiveis { get; } = new();
    public ObservableCollection<ServicoDTO> ServicosDisponiveis { get; } = new();

    public ObservableCollection<ItemPecaOSTemporario> PecasNaOS { get; } = new();
    public ObservableCollection<ItemServicoOSTemporario> ServicosNaOS { get; } = new();

    private ItemPecaOSTemporario? _pecaSelecionadaParaRemover;
    public ItemPecaOSTemporario? PecaSelecionadaParaRemover
    {
        get => _pecaSelecionadaParaRemover;
        set { _pecaSelecionadaParaRemover = value; OnPropertyChanged(); OnPropertyChanged(nameof(TemPecaSelecionada)); }
    }

    public bool TemPecaSelecionada => PecaSelecionadaParaRemover != null;

    private ItemServicoOSTemporario? _servicoSelecionadoParaRemover;
    public ItemServicoOSTemporario? ServicoSelecionadoParaRemover
    {
        get => _servicoSelecionadoParaRemover;
        set { _servicoSelecionadoParaRemover = value; OnPropertyChanged(); OnPropertyChanged(nameof(TemServicoSelecionado)); }
    }

    public bool TemServicoSelecionado => ServicoSelecionadoParaRemover != null;

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

    private int _quantidadePeca = 1;
    public int QuantidadePeca
    {
        get => _quantidadePeca;
        set { _quantidadePeca = value; OnPropertyChanged(); }
    }

    private string _titulo = "Editar Ordem de Serviço";
    public string Titulo
    {
        get => _titulo;
        private set { _titulo = value; OnPropertyChanged(); }
    }

    private string _numeroOS = string.Empty;
    public string NumeroOS
    {
        get => _numeroOS;
        private set { _numeroOS = value; OnPropertyChanged(); }
    }

    private string _dataAbertura = string.Empty;
    public string DataAbertura
    {
        get => _dataAbertura;
        private set { _dataAbertura = value; OnPropertyChanged(); }
    }

    private string _infoCliente = string.Empty;
    public string InfoCliente
    {
        get => _infoCliente;
        private set { _infoCliente = value; OnPropertyChanged(); }
    }

    private string _infoVeiculo = string.Empty;
    public string InfoVeiculo
    {
        get => _infoVeiculo;
        private set { _infoVeiculo = value; OnPropertyChanged(); }
    }

    private string _statusAtual = string.Empty;
    public string StatusAtual
    {
        get => _statusAtual;
        private set { _statusAtual = value; OnPropertyChanged(); }
    }

    private string _observacao = string.Empty;
    public string Observacao
    {
        get => _observacao;
        set { _observacao = value; OnPropertyChanged(); }
    }

    private int? _quilometragem;
    public int? Quilometragem
    {
        get => _quilometragem;
        set { _quilometragem = value; OnPropertyChanged(); }
    }

    private decimal _desconto;
    public decimal Desconto
    {
        get => _desconto;
        set
        {
            _desconto = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ValorTotalGeral));
        }
    }

    public decimal ValorTotalPecas => PecasNaOS.Sum(p => p.ValorTotal);
    public decimal ValorTotalServicos => ServicosNaOS.Sum(s => s.PrecoUnitario);
    public decimal ValorTotalGeral => Math.Max(0, ValorTotalPecas + ValorTotalServicos - (ValorTotalPecas + ValorTotalServicos) * (Desconto / 100));

    public ICommand RemoverPecaCommand { get; }
    public ICommand RemoverServicoCommand { get; }
    public ICommand NovaPecaCommand { get; }
    public ICommand NovoServicoCommand { get; }

    public EditarOSViewModel(IMediator mediator, Guid osId)
    {
        _mediator = mediator;
        _osId = osId;

        RemoverPecaCommand = new RelayCommand(async _ => await RemoverPecaAsync(), _ => TemPecaSelecionada);
        RemoverServicoCommand = new RelayCommand(async _ => await RemoverServicoAsync(), _ => TemServicoSelecionado);
        NovaPecaCommand = new RelayCommand(async _ => await NovaPecaAsync());
        NovoServicoCommand = new RelayCommand(async _ => await NovoServicoAsync());
    }

    public async Task CarregarDadosAsync()
    {
        var os = await _mediator.Send(new ObterOSPorIdQuery { Id = _osId })
            ?? throw new InvalidOperationException("OS não encontrada.");

        Titulo = $"Editar OS Nº {os.Numero}";
        NumeroOS = os.Numero.ToString();
        DataAbertura = os.DataAbertura.ToString("dd/MM/yyyy HH:mm");
        InfoCliente = $"Cliente: {os.NomeCliente} (Tel: {os.TelefoneCliente})";
        InfoVeiculo = $"Veículo: {os.MarcaVeiculo} {os.ModeloVeiculo} - Placa: {os.PlacaVeiculo}";
        StatusAtual = os.Status.ToString();
        Observacao = os.Observacao ?? "";
        Desconto = os.DescontoPercentual;
        Quilometragem = os.Quilometragem;

        PecasNaOS.Clear();
        foreach (var peca in os.ItensPeca)
        {
            var item = new ItemPecaOSTemporario
            {
                Id = peca.Id,
                PecaId = peca.PecaId,
                NomePeca = peca.NomePeca,
                Quantidade = peca.Quantidade,
                PrecoUnitario = peca.PrecoUnitario
            };
            item.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ItemPecaOSTemporario.ValorTotal))
                {
                    OnPropertyChanged(nameof(ValorTotalPecas));
                    OnPropertyChanged(nameof(ValorTotalGeral));
                }
            };
            PecasNaOS.Add(item);
        }

        ServicosNaOS.Clear();
        foreach (var servico in os.ItensServico)
        {
            var item = new ItemServicoOSTemporario
            {
                Id = servico.Id,
                ServicoId = servico.ServicoId,
                NomeServico = servico.NomeServico,
                PrecoUnitario = servico.PrecoUnitario
            };
            item.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ItemServicoOSTemporario.PrecoUnitario))
                {
                    OnPropertyChanged(nameof(ValorTotalServicos));
                    OnPropertyChanged(nameof(ValorTotalGeral));
                }
            };
            ServicosNaOS.Add(item);
        }

        var pecas = await _mediator.Send(new ListarPecasQuery { Ativo = true, TamanhoPagina = 1000 });
        foreach (var p in pecas.Itens)
            PecasDisponiveis.Add(p);

        var servicos = await _mediator.Send(new ListarServicosQuery());
        foreach (var s in servicos)
            ServicosDisponiveis.Add(s);

        OnPropertyChanged(nameof(ValorTotalPecas));
        OnPropertyChanged(nameof(ValorTotalServicos));
        OnPropertyChanged(nameof(ValorTotalGeral));
    }

    public async Task SalvarPrecoPecaAsync(ItemPecaOSTemporario item)
    {
        try
        {
            await _mediator.Send(new AtualizarPrecoItemOSCommand
            {
                OrdemServicoId = _osId,
                ItemId = item.Id,
                EhPeca = true,
                NovoPreco = item.PrecoUnitario
            });

            OnPropertyChanged(nameof(ValorTotalPecas));
            OnPropertyChanged(nameof(ValorTotalGeral));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public async Task SalvarPrecoServicoAsync(ItemServicoOSTemporario item)
    {
        try
        {
            await _mediator.Send(new AtualizarPrecoItemOSCommand
            {
                OrdemServicoId = _osId,
                ItemId = item.Id,
                EhPeca = false,
                NovoPreco = item.PrecoUnitario
            });

            OnPropertyChanged(nameof(ValorTotalServicos));
            OnPropertyChanged(nameof(ValorTotalGeral));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public async Task AdicionarPecaAsync()
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

            await RecarregarItensAsync();
        }
        catch (Exception ex)
        {
            var inner = ex;
            while (inner.InnerException != null)
                inner = inner.InnerException;
            MessageBox.Show($"Erro: {inner.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public async Task AdicionarServicoAsync()
    {
        if (ServicoSelecionado == null) return;

        try
        {
            await _mediator.Send(new AdicionarServicoOSCommand
            {
                OrdemServicoId = _osId,
                ServicoId = ServicoSelecionado.Id
            });

            await RecarregarItensAsync();
        }
        catch (Exception ex)
        {
            var inner = ex;
            while (inner.InnerException != null)
                inner = inner.InnerException;
            MessageBox.Show($"Erro: {inner.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task NovaPecaAsync()
    {
        var dialog = new Views.Pecas.CadastroPecaWindow(
            App.ServiceProvider.GetRequiredService<IMediator>());

        if (dialog.ShowDialog() == true)
        {
            var idsAntes = PecasDisponiveis.Select(p => p.Id).ToHashSet();
            PecasDisponiveis.Clear();
            var pecas = await _mediator.Send(new ListarPecasQuery { Ativo = true, TamanhoPagina = 1000 });
            foreach (var p in pecas.Itens)
                PecasDisponiveis.Add(p);

            var novaPeca = PecasDisponiveis.FirstOrDefault(p => !idsAntes.Contains(p.Id));
            if (novaPeca != null)
                PecaSelecionada = novaPeca;
        }
    }

    private async Task NovoServicoAsync()
    {
        var dialog = new Views.Servicos.CadastroServicoWindow(
            App.ServiceProvider.GetRequiredService<IMediator>());

        if (dialog.ShowDialog() == true)
        {
            var idsAntes = ServicosDisponiveis.Select(s => s.Id).ToHashSet();
            ServicosDisponiveis.Clear();
            var servicos = await _mediator.Send(new ListarServicosQuery());
            foreach (var s in servicos)
                ServicosDisponiveis.Add(s);

            var novoServico = ServicosDisponiveis.FirstOrDefault(s => !idsAntes.Contains(s.Id));
            if (novoServico != null)
                ServicoSelecionado = novoServico;
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

            await RecarregarItensAsync();
        }
        catch (Exception ex)
        {
            var inner = ex;
            while (inner.InnerException != null)
                inner = inner.InnerException;
            MessageBox.Show($"Erro: {inner.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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

            await RecarregarItensAsync();
        }
        catch (Exception ex)
        {
            var inner = ex;
            while (inner.InnerException != null)
                inner = inner.InnerException;
            MessageBox.Show($"Erro: {inner.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task RecarregarItensAsync()
    {
        var os = await _mediator.Send(new ObterOSPorIdQuery { Id = _osId });
        if (os == null) return;

        PecasNaOS.Clear();
        foreach (var peca in os.ItensPeca)
        {
            var item = new ItemPecaOSTemporario
            {
                Id = peca.Id,
                PecaId = peca.PecaId,
                NomePeca = peca.NomePeca,
                Quantidade = peca.Quantidade,
                PrecoUnitario = peca.PrecoUnitario
            };
            item.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ItemPecaOSTemporario.ValorTotal))
                {
                    OnPropertyChanged(nameof(ValorTotalPecas));
                    OnPropertyChanged(nameof(ValorTotalGeral));
                }
            };
            PecasNaOS.Add(item);
        }

        ServicosNaOS.Clear();
        foreach (var servico in os.ItensServico)
        {
            var item = new ItemServicoOSTemporario
            {
                Id = servico.Id,
                ServicoId = servico.ServicoId,
                NomeServico = servico.NomeServico,
                PrecoUnitario = servico.PrecoUnitario
            };
            item.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ItemServicoOSTemporario.PrecoUnitario))
                {
                    OnPropertyChanged(nameof(ValorTotalServicos));
                    OnPropertyChanged(nameof(ValorTotalGeral));
                }
            };
            ServicosNaOS.Add(item);
        }

        OnPropertyChanged(nameof(ValorTotalPecas));
        OnPropertyChanged(nameof(ValorTotalServicos));
        OnPropertyChanged(nameof(ValorTotalGeral));
    }

    public async Task<bool> SalvarAsync()
    {
        try
        {
            await _mediator.Send(new AtualizarObservacaoOSCommand
            {
                OrdemServicoId = _osId,
                Observacao = Observacao
            });

            await _mediator.Send(new AplicarDescontoOSCommand
            {
                OrdemServicoId = _osId,
                DescontoPercentual = Desconto
            });

            await _mediator.Send(new AtualizarQuilometragemOSCommand
            {
                OrdemServicoId = _osId,
                Quilometragem = Quilometragem
            });

            MessageBox.Show("OS atualizada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        catch (Exception ex)
        {
            var inner = ex;
            while (inner.InnerException != null)
                inner = inner.InnerException;
            MessageBox.Show($"Erro: {inner.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class ItemPecaOSTemporario : INotifyPropertyChanged
{
    public Guid Id { get; set; }
    public int PecaId { get; set; }
    public string NomePeca { get; set; } = string.Empty;
    public int Quantidade { get; set; }

    private decimal _precoUnitario;
    public decimal PrecoUnitario
    {
        get => _precoUnitario;
        set
        {
            _precoUnitario = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ValorTotal));
        }
    }

    public decimal ValorTotal => Quantidade * PrecoUnitario;

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class ItemServicoOSTemporario : INotifyPropertyChanged
{
    public Guid Id { get; set; }
    public Guid ServicoId { get; set; }
    public string NomeServico { get; set; } = string.Empty;

    private decimal _precoUnitario;
    public decimal PrecoUnitario
    {
        get => _precoUnitario;
        set
        {
            _precoUnitario = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}