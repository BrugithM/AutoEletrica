using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using SgaAutoEletrica.Application.Features.Clientes.Queries;
using SgaAutoEletrica.Application.Features.Clientes.DTOs;
using SgaAutoEletrica.Application.Features.Veiculos.Queries;
using SgaAutoEletrica.Application.Features.Veiculos.DTOs;
using SgaAutoEletrica.Application.Features.Pecas.Queries;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;
using SgaAutoEletrica.Application.Features.Servicos.Queries;
using SgaAutoEletrica.Application.Features.Servicos.DTOs;
using SgaAutoEletrica.Application.Features.OrdensServico.Commands;

namespace SgaAutoEletrica.UI.ViewModels.OrdensServico;

public class CriacaoOSViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private readonly Guid? _clienteIdPreSelecionado;
    private readonly Guid? _veiculoIdPreSelecionado;

    public ObservableCollection<ClienteResumoDTO> Clientes { get; } = new();
    public ObservableCollection<VeiculoDTO> Veiculos { get; } = new();
    public ObservableCollection<PecaDTO> PecasDisponiveis { get; } = new();
    public ObservableCollection<ServicoDTO> ServicosDisponiveis { get; } = new();

    public ObservableCollection<ItemPecaTemporario> PecasNaOS { get; } = new();
    public ObservableCollection<ItemServicoTemporario> ServicosNaOS { get; } = new();

    private ClienteResumoDTO? _clienteSelecionado;
    public ClienteResumoDTO? ClienteSelecionado
    {
        get => _clienteSelecionado;
        set
        {
            _clienteSelecionado = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(InfoCliente));
            _ = CarregarVeiculosAsync();
        }
    }

    public string InfoCliente => ClienteSelecionado != null 
        ? $"{ClienteSelecionado.NomeCompleto} (Tel: {ClienteSelecionado.Telefone})" 
        : "";

    private VeiculoDTO? _veiculoSelecionado;
    public VeiculoDTO? VeiculoSelecionado
    {
        get => _veiculoSelecionado;
        set
        {
            _veiculoSelecionado = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(InfoVeiculo));
        }
    }

    public string InfoVeiculo => VeiculoSelecionado != null 
        ? $"{VeiculoSelecionado.Marca} {VeiculoSelecionado.Modelo} - Placa: {VeiculoSelecionado.Placa}" 
        : "";

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

    private ItemPecaTemporario? _pecaSelecionadaParaRemover;
    public ItemPecaTemporario? PecaSelecionadaParaRemover
    {
        get => _pecaSelecionadaParaRemover;
        set { _pecaSelecionadaParaRemover = value; OnPropertyChanged(); OnPropertyChanged(nameof(TemPecaSelecionada)); }
    }

    public bool TemPecaSelecionada => PecaSelecionadaParaRemover != null;

    private ItemServicoTemporario? _servicoSelecionadoParaRemover;
    public ItemServicoTemporario? ServicoSelecionadoParaRemover
    {
        get => _servicoSelecionadoParaRemover;
        set { _servicoSelecionadoParaRemover = value; OnPropertyChanged(); OnPropertyChanged(nameof(TemServicoSelecionado)); }
    }

    public bool TemServicoSelecionado => ServicoSelecionadoParaRemover != null;

    private int _quantidadePeca = 1;
    public int QuantidadePeca
    {
        get => _quantidadePeca;
        set { _quantidadePeca = value; OnPropertyChanged(); }
    }

    private string _observacao = string.Empty;
    public string Observacao
    {
        get => _observacao;
        set { _observacao = value; OnPropertyChanged(); }
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
    public decimal ValorTotalServicos => ServicosNaOS.Sum(s => s.Preco);
    public decimal ValorTotalGeral => Math.Max(0, ValorTotalPecas + ValorTotalServicos - Desconto);

    public ICommand RemoverPecaCommand { get; }
    public ICommand RemoverServicoCommand { get; }

    public CriacaoOSViewModel(IMediator mediator, Guid? clienteIdPreSelecionado = null, Guid? veiculoIdPreSelecionado = null)
    {
        _mediator = mediator;
        _clienteIdPreSelecionado = clienteIdPreSelecionado;
        _veiculoIdPreSelecionado = veiculoIdPreSelecionado;

        RemoverPecaCommand = new RelayCommand(_ => RemoverPeca(), _ => TemPecaSelecionada);
        RemoverServicoCommand = new RelayCommand(_ => RemoverServico(), _ => TemServicoSelecionado);
    }

    public async Task CarregarDadosAsync()
    {
        var clientes = await _mediator.Send(new ListarClientesQuery());
        foreach (var c in clientes)
            Clientes.Add(new ClienteResumoDTO { Id = c.Id, NomeCompleto = c.NomeCompleto, Telefone = c.Telefone });

        var pecas = await _mediator.Send(new ListarPecasQuery { Ativo = true });
        foreach (var p in pecas)
            PecasDisponiveis.Add(p);

        var servicos = await _mediator.Send(new ListarServicosQuery());
        foreach (var s in servicos)
            ServicosDisponiveis.Add(s);

        if (_clienteIdPreSelecionado.HasValue)
        {
            ClienteSelecionado = Clientes.FirstOrDefault(c => c.Id == _clienteIdPreSelecionado.Value);
        }
    }

    private async Task CarregarVeiculosAsync()
    {
        Veiculos.Clear();
        if (ClienteSelecionado == null) return;

        var veiculos = await _mediator.Send(new ListarVeiculosPorClienteQuery { ClienteId = ClienteSelecionado.Id });
        foreach (var v in veiculos)
            Veiculos.Add(v);

        if (_veiculoIdPreSelecionado.HasValue)
        {
            VeiculoSelecionado = Veiculos.FirstOrDefault(v => v.Id == _veiculoIdPreSelecionado.Value);
        }
    }

    public void AdicionarPeca()
    {
        if (PecaSelecionada == null || QuantidadePeca <= 0) return;
        if (QuantidadePeca > PecaSelecionada.Estoque)
        {
            MessageBox.Show($"Estoque insuficiente. Disponível: {PecaSelecionada.Estoque}", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var existente = PecasNaOS.FirstOrDefault(p => p.PecaId == PecaSelecionada.Id);
        if (existente != null)
        {
            existente.Quantidade += QuantidadePeca;
        }
        else
        {
            PecasNaOS.Add(new ItemPecaTemporario
            {
                PecaId = PecaSelecionada.Id,
                Nome = PecaSelecionada.Nome,
                Quantidade = QuantidadePeca,
                PrecoUnitario = PecaSelecionada.ValorVenda
            });
        }

        OnPropertyChanged(nameof(ValorTotalPecas));
        OnPropertyChanged(nameof(ValorTotalGeral));
    }

    public void AdicionarServico()
    {
        if (ServicoSelecionado == null) return;

        ServicosNaOS.Add(new ItemServicoTemporario
        {
            ServicoId = ServicoSelecionado.Id,
            Nome = ServicoSelecionado.Nome,
            Preco = ServicoSelecionado.PrecoPadrao
        });

        OnPropertyChanged(nameof(ValorTotalServicos));
        OnPropertyChanged(nameof(ValorTotalGeral));
    }

    private void RemoverPeca()
    {
        if (PecaSelecionadaParaRemover == null) return;
        PecasNaOS.Remove(PecaSelecionadaParaRemover);
        PecaSelecionadaParaRemover = null;
        OnPropertyChanged(nameof(ValorTotalPecas));
        OnPropertyChanged(nameof(ValorTotalGeral));
    }

    private void RemoverServico()
    {
        if (ServicoSelecionadoParaRemover == null) return;
        ServicosNaOS.Remove(ServicoSelecionadoParaRemover);
        ServicoSelecionadoParaRemover = null;
        OnPropertyChanged(nameof(ValorTotalServicos));
        OnPropertyChanged(nameof(ValorTotalGeral));
    }

    public async Task<bool> SalvarAsync(bool aprovarIniciar)
    {
        if (ClienteSelecionado == null)
        {
            MessageBox.Show("Selecione um cliente.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        if (VeiculoSelecionado == null)
        {
            MessageBox.Show("Selecione um veículo.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        if (!PecasNaOS.Any() && !ServicosNaOS.Any())
        {
            MessageBox.Show("Adicione pelo menos uma peça ou serviço.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        try
        {
            var command = new CriarOrdemServicoCommand
            {
                ClienteId = ClienteSelecionado.Id,
                VeiculoId = VeiculoSelecionado.Id,
                Observacao = Observacao,
                Desconto = Desconto,
                AprovarIniciar = aprovarIniciar
            };

            foreach (var item in PecasNaOS)
                command.Pecas.Add(new ItemPecaOSRequest { PecaId = item.PecaId, Quantidade = item.Quantidade });

            foreach (var item in ServicosNaOS)
                command.Servicos.Add(new ItemServicoOSRequest { ServicoId = item.ServicoId });

            await _mediator.Send(command);

            var msg = aprovarIniciar 
                ? "OS criada e iniciada com sucesso!" 
                : "Orçamento emitido com sucesso!";
            MessageBox.Show(msg, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
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

public class ItemPecaTemporario : INotifyPropertyChanged
{
    public Guid PecaId { get; set; }
    public string Nome { get; set; } = string.Empty;

    private int _quantidade;
    public int Quantidade
    {
        get => _quantidade;
        set { _quantidade = value; OnPropertyChanged(); OnPropertyChanged(nameof(ValorTotal)); }
    }

    private decimal _precoUnitario;
    public decimal PrecoUnitario
    {
        get => _precoUnitario;
        set { _precoUnitario = value; OnPropertyChanged(); OnPropertyChanged(nameof(ValorTotal)); }
    }

    public decimal ValorTotal => Quantidade * PrecoUnitario;

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class ItemServicoTemporario
{
    public Guid ServicoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
}