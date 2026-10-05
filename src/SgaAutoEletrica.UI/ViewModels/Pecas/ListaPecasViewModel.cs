using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Common.Interfaces;
using SgaAutoEletrica.Application.Features.CategoriasPeca.DTOs;
using SgaAutoEletrica.Application.Features.CategoriasPeca.Queries;
using SgaAutoEletrica.Application.Features.Fornecedores.DTOs;
using SgaAutoEletrica.Application.Features.Fornecedores.Queries;
using SgaAutoEletrica.Application.Features.Pecas.Commands;
using SgaAutoEletrica.Application.Features.Pecas.DTOs;
using SgaAutoEletrica.Application.Features.Pecas.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Pecas;

public class ListaPecasViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private const int TamanhoPagina = 20;

    public ObservableCollection<PecaDTO> Pecas { get; } = new();
    public ObservableCollection<CategoriaPecaDTO> Categorias { get; } = new();
    public ObservableCollection<FornecedorDTO> Fornecedores { get; } = new();

    public bool EhAdministrador => App.ServiceProvider
        .GetRequiredService<ISessaoUsuario>().EhAdministrador;

    private PecaDTO? _pecaSelecionada;
    public PecaDTO? PecaSelecionada
    {
        get => _pecaSelecionada;
        set
        {
            _pecaSelecionada = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TemPecaSelecionada));
            OnPropertyChanged(nameof(TemFornecedor));
            OnPropertyChanged(nameof(FornecedorNomeExibir));
            OnPropertyChanged(nameof(FornecedorCnpjExibir));
            OnPropertyChanged(nameof(FornecedorTelefoneExibir));
            OnPropertyChanged(nameof(FornecedorContatoExibir));
            OnPropertyChanged(nameof(TextoBotaoDesativar));
        }
    }

    public bool TemPecaSelecionada => PecaSelecionada != null;
    public bool TemFornecedor => !string.IsNullOrWhiteSpace(PecaSelecionada?.FornecedorNome);

    public string FornecedorNomeExibir => PecaSelecionada?.FornecedorNome ?? "";
    public string FornecedorCnpjExibir => PecaSelecionada?.FornecedorCnpj ?? "";
    public string FornecedorTelefoneExibir => PecaSelecionada?.FornecedorTelefone ?? "";
    public string FornecedorContatoExibir => PecaSelecionada?.FornecedorContato ?? "";

    public string TextoBotaoDesativar =>
        PecaSelecionada?.Ativo == false ? "▶️ Reativar" : "⏸️ Desativar";

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

    private string _termoBusca = string.Empty;
    public string TermoBusca
    {
        get => _termoBusca;
        set { _termoBusca = value; OnPropertyChanged(); }
    }

    private string _buscaNome = string.Empty;
    public string BuscaNome
    {
        get => _buscaNome;
        set { _buscaNome = value; OnPropertyChanged(); }
    }

    private string _buscaCodigoPeca = string.Empty;
    public string BuscaCodigoPeca
    {
        get => _buscaCodigoPeca;
        set { _buscaCodigoPeca = value; OnPropertyChanged(); }
    }

    private string _buscaCodigoBarras = string.Empty;
    public string BuscaCodigoBarras
    {
        get => _buscaCodigoBarras;
        set { _buscaCodigoBarras = value; OnPropertyChanged(); }
    }

    private string _buscaIdPeca = string.Empty;
    public string BuscaIdPeca
    {
        get => _buscaIdPeca;
        set { _buscaIdPeca = value; OnPropertyChanged(); }
    }

    private string _buscaMarca = string.Empty;
    public string BuscaMarca
    {
        get => _buscaMarca;
        set { _buscaMarca = value; OnPropertyChanged(); }
    }

    private CategoriaPecaDTO? _buscaCategoria;
    public CategoriaPecaDTO? BuscaCategoria
    {
        get => _buscaCategoria;
        set { _buscaCategoria = value; OnPropertyChanged(); }
    }

    private FornecedorDTO? _buscaFornecedor;
    public FornecedorDTO? BuscaFornecedor
    {
        get => _buscaFornecedor;
        set { _buscaFornecedor = value; OnPropertyChanged(); }
    }

    private bool _buscaApenasEstoqueBaixo;
    public bool BuscaApenasEstoqueBaixo
    {
        get => _buscaApenasEstoqueBaixo;
        set { _buscaApenasEstoqueBaixo = value; OnPropertyChanged(); }
    }

    private bool _mostrarInativos;
    public bool MostrarInativos
    {
        get => _mostrarInativos;
        set { _mostrarInativos = value; OnPropertyChanged(); _ = BuscarAsync(); }
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
        $"Página {PaginaAtual} de {TotalPaginas}  |  Total: {TotalItens} peças";

    public bool TemPaginaAnterior => PaginaAtual > 1;
    public bool TemProximaPagina => PaginaAtual < TotalPaginas;

    public ICommand BuscarCommand { get; }
    public ICommand LimparCommand { get; }
    public ICommand AlternarModoBuscaCommand { get; }
    public ICommand NovaPecaCommand { get; }
    public ICommand EditarPecaCommand { get; }
    public ICommand DesativarPecaCommand { get; }
    public ICommand MovimentarEstoqueCommand { get; }
    public ICommand AtualizarCommand { get; }
    public ICommand AbrirFornecedorCommand { get; }
    public ICommand PaginaAnteriorCommand { get; }
    public ICommand ProximaPaginaCommand { get; }

    public ListaPecasViewModel(IMediator mediator)
    {
        _mediator = mediator;

        BuscarCommand = new RelayCommand(async _ => { PaginaAtual = 1; await BuscarAsync(); });
        LimparCommand = new RelayCommand(async _ => { LimparCampos(); PaginaAtual = 1; await BuscarAsync(); });
        AlternarModoBuscaCommand = new RelayCommand(_ => AlternarModo());
        NovaPecaCommand = new RelayCommand(async _ => await NovaPecaAsync());
        EditarPecaCommand = new RelayCommand(async _ => await EditarPecaAsync(), _ => TemPecaSelecionada);
        DesativarPecaCommand = new RelayCommand(async _ => await DesativarPecaAsync(), _ => TemPecaSelecionada && EhAdministrador);
        MovimentarEstoqueCommand = new RelayCommand(async _ => await MovimentarEstoqueAsync(), _ => TemPecaSelecionada && EhAdministrador);
        AtualizarCommand = new RelayCommand(async _ => await BuscarAsync());
        AbrirFornecedorCommand = new RelayCommand(_ => AbrirFornecedor(), _ => TemFornecedor);
        PaginaAnteriorCommand = new RelayCommand(async _ => await IrParaPaginaAnterior(), _ => TemPaginaAnterior);
        ProximaPaginaCommand = new RelayCommand(async _ => await IrParaProximaPagina(), _ => TemProximaPagina);
    }

    public async Task CarregarDadosAuxiliaresAsync()
    {
        Categorias.Clear();
        Categorias.Add(new CategoriaPecaDTO { Id = Guid.Empty, Nome = "Todas" });
        var cats = await _mediator.Send(new ListarCategoriasPecaQuery());
        foreach (var cat in cats)
            Categorias.Add(cat);

        Fornecedores.Clear();
        Fornecedores.Add(new FornecedorDTO { Id = Guid.Empty, NomeEmpresa = "Todos" });
        var forns = await _mediator.Send(new ListarFornecedoresQuery());
        foreach (var forn in forns)
            Fornecedores.Add(forn);

        BuscaCategoria = Categorias.FirstOrDefault();
        BuscaFornecedor = Fornecedores.FirstOrDefault();
    }

    public async Task BuscarAsync()
    {
        try
        {
            Pecas.Clear();
            PecaSelecionada = null;

            var resultado = await _mediator.Send(new ListarPecasQuery
            {
                TermoBusca = ModoBuscaRapida ? TermoBusca : null,
                Nome = ModoBuscaAvancada ? BuscaNome : null,
                CodigoPeca = ModoBuscaAvancada ? BuscaCodigoPeca : null,
                CodigoBarras = ModoBuscaAvancada ? BuscaCodigoBarras : null,
                IdPeca = ModoBuscaAvancada ? BuscaIdPeca : null,
                Marca = ModoBuscaAvancada ? BuscaMarca : null,
                CategoriaId = ModoBuscaAvancada ? BuscaCategoria?.Id : null,
                FornecedorId = ModoBuscaAvancada ? BuscaFornecedor?.Id : null,
                ApenasEstoqueBaixo = ModoBuscaAvancada && BuscaApenasEstoqueBaixo,
                Ativo = MostrarInativos ? null : true,
                Pagina = PaginaAtual,
                TamanhoPagina = TamanhoPagina
            });

            foreach (var peca in resultado.Itens)
                Pecas.Add(peca);

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
        BuscaCodigoPeca = string.Empty;
        BuscaCodigoBarras = string.Empty;
        BuscaIdPeca = string.Empty;
        BuscaMarca = string.Empty;
        BuscaApenasEstoqueBaixo = false;
        BuscaCategoria = Categorias.FirstOrDefault();
        BuscaFornecedor = Fornecedores.FirstOrDefault();
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

    private async Task DesativarPecaAsync()
    {
        if (PecaSelecionada == null) return;

        var acao = PecaSelecionada.Ativo ? "desativar" : "reativar";
        var confirmacao = MessageBox.Show(
            $"Deseja realmente {acao} a peça '{PecaSelecionada.Nome}'?",
            "Confirmar",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirmacao == MessageBoxResult.Yes)
        {
            if (PecaSelecionada.Ativo)
                await _mediator.Send(new DesativarPecaCommand { Id = PecaSelecionada.Id });
            else
                await _mediator.Send(new ReativarPecaCommand { Id = PecaSelecionada.Id });

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

    private void AbrirFornecedor()
    {
        if (PecaSelecionada?.FornecedorId == null) return;
        MessageBox.Show($"Abrir fornecedor {PecaSelecionada.FornecedorNome} — em breve", "Em breve");
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