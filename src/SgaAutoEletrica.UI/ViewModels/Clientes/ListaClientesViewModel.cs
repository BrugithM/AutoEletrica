using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Application.Features.Clientes.Commands;
using SgaAutoEletrica.Application.Features.Clientes.DTOs;
using SgaAutoEletrica.Application.Features.Clientes.Queries;
using SgaAutoEletrica.UI.Views.Clientes;

namespace SgaAutoEletrica.UI.ViewModels.Clientes;

public class ListaClientesViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private const int TamanhoPagina = 20;

    public ObservableCollection<ClienteDTO> Clientes { get; } = new();
    public ObservableCollection<VeiculoResumoDTO> VeiculosDoCliente { get; } = new();

    public bool EhAdministrador => App.ServiceProvider
        .GetRequiredService<ISessaoUsuario>().EhAdministrador;

    // ─── Cliente selecionado ───
    private ClienteDTO? _clienteSelecionado;
    public ClienteDTO? ClienteSelecionado
    {
        get => _clienteSelecionado;
        set
        {
            _clienteSelecionado = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TemClienteSelecionado));
            OnPropertyChanged(nameof(TextoBotaoDesativar));
            _ = CarregarVeiculosAsync();
        }
    }

    public bool TemClienteSelecionado => ClienteSelecionado != null;

    public string TextoBotaoDesativar =>
        ClienteSelecionado?.Ativo == false ? "▶️ Reativar" : "⏸️ Desativar";

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
    private string _buscaNome = string.Empty;
    public string BuscaNome
    {
        get => _buscaNome;
        set { _buscaNome = value; OnPropertyChanged(); }
    }

    private string _buscaCpf = string.Empty;
    public string BuscaCpf
    {
        get => _buscaCpf;
        set { _buscaCpf = value; OnPropertyChanged(); }
    }

    private string _buscaTelefone = string.Empty;
    public string BuscaTelefone
    {
        get => _buscaTelefone;
        set { _buscaTelefone = value; OnPropertyChanged(); }
    }

    private string _buscaEndereco = string.Empty;
    public string BuscaEndereco
    {
        get => _buscaEndereco;
        set { _buscaEndereco = value; OnPropertyChanged(); }
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
        $"Página {PaginaAtual} de {TotalPaginas}  |  Total: {TotalItens} clientes";

    public bool TemPaginaAnterior => PaginaAtual > 1;
    public bool TemProximaPagina => PaginaAtual < TotalPaginas;

    // ─── Comandos ───
    public ICommand BuscarCommand { get; }
    public ICommand LimparBuscaCommand { get; }
    public ICommand AlternarModoBuscaCommand { get; }
    public ICommand NovoClienteCommand { get; }
    public ICommand EditarClienteCommand { get; }
    public ICommand DesativarClienteCommand { get; }
    public ICommand AtualizarCommand { get; }
    public ICommand VincularVeiculoCommand { get; }
    public ICommand PaginaAnteriorCommand { get; }
    public ICommand ProximaPaginaCommand { get; }

    public ListaClientesViewModel(IMediator mediator)
    {
        _mediator = mediator;

        BuscarCommand = new RelayCommand(async _ => { PaginaAtual = 1; await BuscarAsync(); });
        LimparBuscaCommand = new RelayCommand(async _ => { LimparCampos(); PaginaAtual = 1; await BuscarAsync(); });
        AlternarModoBuscaCommand = new RelayCommand(_ => AlternarModo());
        NovoClienteCommand = new RelayCommand(async _ => await NovoClienteAsync());
        EditarClienteCommand = new RelayCommand(async _ => await EditarClienteAsync(), _ => TemClienteSelecionado);
        DesativarClienteCommand = new RelayCommand(async _ => await DesativarClienteAsync(), _ => TemClienteSelecionado && EhAdministrador);
        AtualizarCommand = new RelayCommand(async _ => await BuscarAsync());
        VincularVeiculoCommand = new RelayCommand(async _ => await VincularVeiculoAsync(), _ => TemClienteSelecionado);
        PaginaAnteriorCommand = new RelayCommand(async _ => await IrParaPaginaAnterior(), _ => TemPaginaAnterior);
        ProximaPaginaCommand = new RelayCommand(async _ => await IrParaProximaPagina(), _ => TemProximaPagina);
    }

    public async Task BuscarAsync()
    {
        try
        {
            Clientes.Clear();
            VeiculosDoCliente.Clear();
            ClienteSelecionado = null;

            var resultado = await _mediator.Send(new ListarClientesQuery
            {
                TermoBusca = ModoBuscaRapida ? TermoBusca : null,
                Nome = ModoBuscaAvancada ? BuscaNome : null,
                Cpf = ModoBuscaAvancada ? BuscaCpf : null,
                Telefone = ModoBuscaAvancada ? BuscaTelefone : null,
                Endereco = ModoBuscaAvancada ? BuscaEndereco : null,
                Ativo = MostrarInativos ? null : true,
                Pagina = PaginaAtual,
                TamanhoPagina = TamanhoPagina
            });

            foreach (var cliente in resultado.Itens)
                Clientes.Add(cliente);

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
        BuscaNome = string.Empty;
        BuscaCpf = string.Empty;
        BuscaTelefone = string.Empty;
        BuscaEndereco = string.Empty;
    }

    private async Task CarregarVeiculosAsync()
    {
        VeiculosDoCliente.Clear();
        if (ClienteSelecionado == null) return;

        var cliente = await _mediator.Send(new ObterClienteComVeiculosQuery { ClienteId = ClienteSelecionado.Id });
        if (cliente != null)
        {
            foreach (var veiculo in cliente.Veiculos)
                VeiculosDoCliente.Add(veiculo);
        }
    }

    private async Task NovoClienteAsync()
    {
        var dialog = new CadastroClienteWindow(_mediator);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    private async Task EditarClienteAsync()
    {
        if (ClienteSelecionado == null) return;
        var dialog = new CadastroClienteWindow(_mediator, ClienteSelecionado.Id);
        dialog.ShowDialog();
        await BuscarAsync();
    }

    private async Task DesativarClienteAsync()
    {
        if (ClienteSelecionado == null) return;

        var acao = ClienteSelecionado.Ativo ? "desativar" : "reativar";
        var confirmacao = MessageBox.Show(
            $"Deseja realmente {acao} o cliente '{ClienteSelecionado.NomeCompleto}'?",
            "Confirmar",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirmacao == MessageBoxResult.Yes)
        {
            if (ClienteSelecionado.Ativo)
                await _mediator.Send(new DesativarClienteCommand { Id = ClienteSelecionado.Id });
            else
                await _mediator.Send(new ReativarClienteCommand { Id = ClienteSelecionado.Id });

            await BuscarAsync();
        }
    }

    private async Task VincularVeiculoAsync()
    {
        if (ClienteSelecionado == null) return;

        var dialog = new SgaAutoEletrica.UI.Views.Veiculos.CadastroVeiculoWindow(_mediator, null, ClienteSelecionado.Id);
        dialog.ShowDialog();
        await CarregarVeiculosAsync();
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