using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SgaAutoEletrica.Application.Features.CategoriasPeca.DTOs;
using SgaAutoEletrica.Application.Features.CategoriasPeca.Queries;
using SgaAutoEletrica.Application.Features.Fornecedores.DTOs;
using SgaAutoEletrica.Application.Features.Fornecedores.Queries;
using SgaAutoEletrica.Application.Features.Marcas.DTOs;
using SgaAutoEletrica.Application.Features.Marcas.Queries;
using SgaAutoEletrica.Application.Features.Pecas.Commands;
using SgaAutoEletrica.Application.Features.Pecas.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Pecas;

public class CadastroPecaViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private readonly int? _pecaId;

    public string CodigoPeca { get; set; } = string.Empty;
    public string CodigoBarras { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int EstoqueInicial { get; set; }
    public int EstoqueMinimo { get; set; } = 5;

    private decimal _valorCusto;
    public decimal ValorCusto
    {
        get => _valorCusto;
        set
        {
            _valorCusto = value;
            OnPropertyChanged();
            RecalcularVendaPorMargem();
        }
    }

    private decimal _valorVenda;
    public decimal ValorVenda
    {
        get => _valorVenda;
        set
        {
            _valorVenda = value;
            OnPropertyChanged();
            RecalcularMargemPorVenda();
        }
    }

    private decimal _margemLucro;
    public decimal MargemLucro
    {
        get => _margemLucro;
        set
        {
            _margemLucro = value;
            OnPropertyChanged();
            RecalcularVendaPorMargem();
        }
    }

    private decimal _markup;
    public decimal Markup
    {
        get => _markup;
        set
        {
            _markup = value;
            OnPropertyChanged();
            RecalcularVendaPorMarkup();
        }
    }

    private bool _recalculando;

    public ObservableCollection<CategoriaPecaDTO> Categorias { get; } = new();
    public ObservableCollection<FornecedorDTO> Fornecedores { get; } = new();
    public ObservableCollection<MarcaDTO> Marcas { get; } = new();

    private CategoriaPecaDTO? _categoriaSelecionada;
    public CategoriaPecaDTO? CategoriaSelecionada
    {
        get => _categoriaSelecionada;
        set { _categoriaSelecionada = value; OnPropertyChanged(); }
    }

    private FornecedorDTO? _fornecedorSelecionado;
    public FornecedorDTO? FornecedorSelecionado
    {
        get => _fornecedorSelecionado;
        set { _fornecedorSelecionado = value; OnPropertyChanged(); }
    }

    private MarcaDTO? _marcaSelecionada;
    public MarcaDTO? MarcaSelecionada
    {
        get => _marcaSelecionada;
        set { _marcaSelecionada = value; OnPropertyChanged(); }
    }

    private string _titulo = "Cadastro de Peças";
    public string Titulo
    {
        get => _titulo;
        private set { _titulo = value; OnPropertyChanged(); }
    }

    public bool EhAdministrador => App.ServiceProvider
        .GetRequiredService<SgaAutoEletrica.Application.Common.Interfaces.ISessaoUsuario>().EhAdministrador;

    public ICommand NovaCategoriaCommand { get; }
    public ICommand NovaMarcaCommand { get; }

    public CadastroPecaViewModel(IMediator mediator, int? pecaId = null)
    {
        _mediator = mediator;
        _pecaId = pecaId;

        NovaCategoriaCommand = new RelayCommand(async _ => await NovaCategoriaAsync(), _ => EhAdministrador);
        NovaMarcaCommand = new RelayCommand(async _ => await NovaMarcaAsync(), _ => EhAdministrador);

        if (pecaId.HasValue)
        {
            Titulo = "Editar Peça";
            CarregarDadosAsync(pecaId.Value);
        }
    }

    private async void CarregarDadosAsync(int pecaId)
    {
        var peca = await _mediator.Send(new ObterPecaPorIdQuery { Id = pecaId });
        if (peca != null)
        {
            _recalculando = true;

            CodigoPeca = peca.CodigoPeca ?? "";
            CodigoBarras = peca.CodigoBarras ?? "";
            Nome = peca.Nome;
            Descricao = peca.Descricao;
            ValorCusto = peca.ValorCusto;
            ValorVenda = peca.ValorVenda;
            MargemLucro = peca.MargemLucro;
            Markup = peca.MarkupPercentual;
            EstoqueInicial = peca.Estoque;
            EstoqueMinimo = peca.EstoqueMinimo;

            _recalculando = false;

            _marcaIdParaSelecionar = peca.MarcaId;
            _categoriaIdParaSelecionar = peca.CategoriaId;
            _fornecedorIdParaSelecionar = peca.FornecedorId;

            OnPropertyChanged(nameof(CodigoPeca));
            OnPropertyChanged(nameof(CodigoBarras));
            OnPropertyChanged(nameof(Nome));
            OnPropertyChanged(nameof(Descricao));
            OnPropertyChanged(nameof(ValorCusto));
            OnPropertyChanged(nameof(ValorVenda));
            OnPropertyChanged(nameof(MargemLucro));
            OnPropertyChanged(nameof(Markup));
            OnPropertyChanged(nameof(EstoqueInicial));
            OnPropertyChanged(nameof(EstoqueMinimo));
        }
    }

    private int? _marcaIdParaSelecionar;
    private Guid? _categoriaIdParaSelecionar;
    private Guid? _fornecedorIdParaSelecionar;

    private void RecalcularVendaPorMargem()
    {
        if (_recalculando) return;
        if (ValorCusto <= 0 || MargemLucro <= 0 || MargemLucro >= 100) return;

        _recalculando = true;
        ValorVenda = Math.Round(ValorCusto / (1 - MargemLucro / 100), 2);
        Markup = ValorCusto > 0 ? Math.Round((ValorVenda - ValorCusto) / ValorCusto * 100, 2) : 0;
        _recalculando = false;

        OnPropertyChanged(nameof(ValorVenda));
        OnPropertyChanged(nameof(Markup));
    }

    private void RecalcularVendaPorMarkup()
    {
        if (_recalculando) return;
        if (ValorCusto <= 0 || Markup <= 0) return;

        _recalculando = true;
        ValorVenda = Math.Round(ValorCusto * (1 + Markup / 100), 2);
        MargemLucro = ValorVenda > 0 ? Math.Round((ValorVenda - ValorCusto) / ValorVenda * 100, 2) : 0;
        _recalculando = false;

        OnPropertyChanged(nameof(ValorVenda));
        OnPropertyChanged(nameof(MargemLucro));
    }

    private void RecalcularMargemPorVenda()
    {
        if (_recalculando) return;
        if (ValorVenda <= 0) return;

        _recalculando = true;
        MargemLucro = Math.Round((ValorVenda - ValorCusto) / ValorVenda * 100, 2);
        Markup = ValorCusto > 0 ? Math.Round((ValorVenda - ValorCusto) / ValorCusto * 100, 2) : 0;
        _recalculando = false;

        OnPropertyChanged(nameof(MargemLucro));
        OnPropertyChanged(nameof(Markup));
    }

    public async Task CarregarDadosAuxiliaresAsync()
    {
        Categorias.Clear();
        var categorias = await _mediator.Send(new ListarCategoriasPecaQuery());
        foreach (var cat in categorias)
            Categorias.Add(cat);

        Fornecedores.Clear();
        var fornecedores = await _mediator.Send(new ListarFornecedoresQuery());
        foreach (var forn in fornecedores)
            Fornecedores.Add(forn);

        Marcas.Clear();
        var marcas = await _mediator.Send(new ListarMarcasQuery());
        foreach (var m in marcas)
            Marcas.Add(m);

        if (_marcaIdParaSelecionar.HasValue)
            MarcaSelecionada = Marcas.FirstOrDefault(m => m.Id == _marcaIdParaSelecionar.Value);

        if (_categoriaIdParaSelecionar.HasValue)
            CategoriaSelecionada = Categorias.FirstOrDefault(c => c.Id == _categoriaIdParaSelecionar.Value);

        if (_fornecedorIdParaSelecionar.HasValue)
            FornecedorSelecionado = Fornecedores.FirstOrDefault(f => f.Id == _fornecedorIdParaSelecionar.Value);
    }

    private async Task NovaCategoriaAsync()
    {
        var dialog = new Views.CategoriasPeca.CadastroCategoriaWindow(_mediator);
        if (dialog.ShowDialog() == true)
        {
            var nomeAntes = Categorias.Select(c => c.Nome).ToHashSet();
            Categorias.Clear();
            var categorias = await _mediator.Send(new ListarCategoriasPecaQuery());
            foreach (var cat in categorias)
                Categorias.Add(cat);

            var novaCategoria = Categorias.FirstOrDefault(c => !nomeAntes.Contains(c.Nome));
            if (novaCategoria != null)
                CategoriaSelecionada = novaCategoria;
        }
    }

    private async Task NovaMarcaAsync()
    {
        var dialog = new Views.Marcas.CadastroMarcaWindow(_mediator);
        if (dialog.ShowDialog() == true)
        {
            var nomeAntes = Marcas.Select(m => m.Nome).ToHashSet();
            Marcas.Clear();
            var marcas = await _mediator.Send(new ListarMarcasQuery());
            foreach (var m in marcas)
                Marcas.Add(m);

            var novaMarca = Marcas.FirstOrDefault(m => !nomeAntes.Contains(m.Nome));
            if (novaMarca != null)
                MarcaSelecionada = novaMarca;
        }
    }

    public async Task<bool> SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(Nome))
        {
            MessageBox.Show("Nome é obrigatório.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        if (string.IsNullOrWhiteSpace(Descricao))
        {
            MessageBox.Show("Descrição é obrigatória.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        try
        {
            if (_pecaId.HasValue)
            {
                await _mediator.Send(new AtualizarPecaCommand
                {
                    Id = _pecaId.Value,
                    Nome = Nome,
                    Descricao = Descricao,
                    ValorCusto = ValorCusto,
                    ValorVenda = ValorVenda,
                    CodigoPeca = CodigoPeca,
                    MarcaId = MarcaSelecionada?.Id,
                    CategoriaId = CategoriaSelecionada?.Id,
                    FornecedorId = FornecedorSelecionado?.Id,
                    EstoqueMinimo = EstoqueMinimo
                });
            }
            else
            {
                await _mediator.Send(new CriarPecaCommand
                {
                    Nome = Nome,
                    Descricao = Descricao,
                    ValorCusto = ValorCusto,
                    ValorVenda = ValorVenda,
                    EstoqueInicial = EstoqueInicial,
                    EstoqueMinimo = EstoqueMinimo,
                    CodigoPeca = CodigoPeca,
                    CodigoBarras = CodigoBarras,
                    MarcaId = MarcaSelecionada?.Id,
                    CategoriaId = CategoriaSelecionada?.Id,
                    FornecedorId = FornecedorSelecionado?.Id
                });
            }

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

    public void Limpar()
    {
        CodigoPeca = string.Empty;
        CodigoBarras = string.Empty;
        Nome = string.Empty;
        Descricao = string.Empty;
        ValorCusto = 0;
        ValorVenda = 0;
        MargemLucro = 0;
        Markup = 0;
        EstoqueInicial = 0;
        EstoqueMinimo = 5;
        MarcaSelecionada = null;
        CategoriaSelecionada = null;
        FornecedorSelecionado = null;

        OnPropertyChanged(nameof(CodigoPeca));
        OnPropertyChanged(nameof(CodigoBarras));
        OnPropertyChanged(nameof(Nome));
        OnPropertyChanged(nameof(Descricao));
        OnPropertyChanged(nameof(ValorCusto));
        OnPropertyChanged(nameof(ValorVenda));
        OnPropertyChanged(nameof(MargemLucro));
        OnPropertyChanged(nameof(Markup));
        OnPropertyChanged(nameof(EstoqueInicial));
        OnPropertyChanged(nameof(EstoqueMinimo));
        OnPropertyChanged(nameof(MarcaSelecionada));
        OnPropertyChanged(nameof(CategoriaSelecionada));
        OnPropertyChanged(nameof(FornecedorSelecionado));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}