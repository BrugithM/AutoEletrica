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
    private const int TamanhoPagina = 20;

    public ObservableCollection<VeiculoDTO> Veiculos { get; } = new();

    public bool EhAdministrador => App.ServiceProvider
        .GetRequiredService<ISessaoUsuario>().EhAdministrador;

    // ─── Veículo selecionado ───
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

    // ─── Modo de busca ───
    private bool _modoBuscaAvancada;
    public bool ModoBuscaAvancada
    {
        get => _modoBuscaAvancada;
        set
        {
            _modoBuscaAvancada = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ModoBuscaRapida));
        }
    }

    public bool ModoBuscaRapida => !ModoBuscaAvancada;

    // ─── Busca rápida ───
    private string _termoBusca = string.Empty;
    public string TermoBusca
    {
        get => _termoBusca;
        set { _termoBusca = value; OnPropertyChanged(); }
    }

    // ─── Busca avançada ───
    private string _buscaPlaca = string.Empty;
    public string BuscaPlaca
    {
        get => _buscaPlaca;
        set { _buscaPlaca = value; OnPropertyChanged(); }
    }

    private string _buscaModelo = string.Empty;
    public string BuscaModelo
    {
        get => _buscaModelo;
        set { _buscaModelo = value; OnPropertyChanged(); }
    }

    private string _buscaMarca = string.Empty;
    public string BuscaMarca
    {
        get => _buscaMarca;
        set { _buscaMarca = value; OnPropertyChanged(); }
    }

    private string _buscaNomeCliente = string.Empty;
    public string BuscaNomeCliente
    {
        get => _buscaNomeCliente;
        set { _buscaNomeCliente = value; OnPropertyChanged(); }
    }

    private string _buscaAno = string.Empty;
    public string BuscaAno
    {
        get => _buscaAno;
        set { _buscaAno = value; OnPropertyChanged(); }
    }

    // ─── Filtros comuns ───
    private bool _mostrarInativos;
    public bool MostrarInativos
    {
        get => _mostrarInativos;
        set { _mostrarInativos = value; OnPropertyChanged(); _ = BuscarAsync(); }
    }

    // ─── Paginação ───
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
        $"Página {PaginaAtual} de {TotalPaginas}  |  Total: {TotalItens} veículos";

    public bool TemPaginaAnterior => PaginaAtual > 1;
    public bool TemProximaPagina => PaginaAtual < TotalPaginas;

    // ─── Comandos ───
    public ICommand BuscarCommand { get; }
    public ICommand LimparCommand { get; }
    public ICommand AlternarModoBuscaCommand { get; }
    public ICommand NovoVeiculoCommand { get; }
    public ICommand EditarVeiculoCommand { get; }
    public ICommand DesativarVeiculoCommand { get; }
    public ICommand AtualizarCommand { get; }
    public ICommand NFsVinculadasCommand { get; }
    public ICommand CriarOSCommand { get; }
    public ICommand PaginaAnteriorCommand { get; }
    public ICommand ProximaPaginaCommand { get; }

    public ListaVeiculosViewModel(IMediator mediator)
    {
        _mediator = mediator;

        BuscarCommand = new RelayCommand(async _ => { PaginaAtual = 1; await BuscarAsync(); });
        LimparCommand = new RelayCommand(async _ => { LimparCampos(); PaginaAtual = 1; await BuscarAsync(); });
        AlternarModoBuscaCommand = new RelayCommand(_ => AlternarModo());
        NovoVeiculoCommand = new RelayCommand(async _ => await NovoVeiculoAsync());
        EditarVeiculoCommand = new RelayCommand(async _ => await EditarVeiculoAsync(), _ => TemVeiculoSelecionado);
        DesativarVeiculoCommand = new RelayCommand(async _ => await DesativarVeiculoAsync(), _ => TemVeiculoSelecionado && EhAdministrador);
        AtualizarCommand = new RelayCommand(async _ => await BuscarAsync());
        NFsVinculadasCommand = new RelayCommand(_ => NFsVinculadas(), _ => TemVeiculoSelecionado);
        CriarOSCommand = new RelayCommand(_ => CriarOS(), _ => TemVeiculoSelecionado);
        PaginaAnteriorCommand = new RelayCommand(async _ => await IrParaPaginaAnterior(), _ => TemPaginaAnterior);
        ProximaPaginaCommand = new RelayCommand(async _ => await IrParaProximaPagina(), _ => TemProximaPagina);
    }

    public async Task BuscarAsync()
    {
        try
        {
            Veiculos.Clear();
            VeiculoSelecionado = null;

            int? ano = null;
            if (!string.IsNullOrWhiteSpace(BuscaAno) && int.TryParse(BuscaAno, out var anoParsed))
                ano = anoParsed;

            var resultado = await _mediator.Send(new BuscarVeiculosQuery
            {
                TermoBusca = ModoBuscaRapida ? TermoBusca : null,
                Placa = ModoBuscaAvancada ? BuscaPlaca : null,
                Modelo = ModoBuscaAvancada ? BuscaModelo : null,
                Marca = ModoBuscaAvancada ? BuscaMarca : null,
                NomeCliente = ModoBuscaAvancada ? BuscaNomeCliente : null,
                Ano = ModoBuscaAvancada ? ano : null,
                Ativo = MostrarInativos ? null : true,
                Pagina = PaginaAtual,
                TamanhoPagina = TamanhoPagina
            });

            foreach (var veiculo in resultado.Itens)
                Veiculos.Add(veiculo);

            TotalItens = resultado.TotalItens;
            TotalPaginas = resultado.TotalPaginas;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void AlternarModo()
    {
        ModoBuscaAvancada = !ModoBuscaAvancada;
        LimparCampos();
    }

    private void LimparCampos()
    {
        TermoBusca = string.Empty;
        BuscaPlaca = string.Empty;
        BuscaModelo = string.Empty;
        BuscaMarca = string.Empty;
        BuscaNomeCliente = string.Empty;
        BuscaAno = string.Empty;
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

    private void NFsVinculadas()
    {
        if (VeiculoSelecionado == null) return;
        MessageBox.Show($"Lista de NFs do veículo {VeiculoSelecionado.Placa} - em breve", "Em breve");
    }

    private void CriarOS()
    {
        if (VeiculoSelecionado == null) return;
        var dialog = new Views.OrdensServico.CriacaoOSWindow(
            _mediator,
            VeiculoSelecionado.ClienteId,
            VeiculoSelecionado.Id);
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