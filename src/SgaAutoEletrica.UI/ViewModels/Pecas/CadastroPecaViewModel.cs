using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MediatR;
using SgaAutoEletrica.Application.Features.CategoriasPeca.DTOs;
using SgaAutoEletrica.Application.Features.CategoriasPeca.Queries;
using SgaAutoEletrica.Application.Features.Fornecedores.DTOs;
using SgaAutoEletrica.Application.Features.Fornecedores.Queries;
using SgaAutoEletrica.Application.Features.Pecas.Commands;
using SgaAutoEletrica.Application.Features.Pecas.Queries;

namespace SgaAutoEletrica.UI.ViewModels.Pecas;

public class CadastroPecaViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private readonly Guid? _pecaId;

    public string IdPeca { get; set; } = string.Empty;
    public string CodigoPeca { get; set; } = string.Empty;
    public string CodigoBarras { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public decimal ValorCusto { get; set; }
    public decimal ValorVenda { get; set; }
    public decimal Imposto { get; set; }
    public int EstoqueInicial { get; set; }
    public int EstoqueMinimo { get; set; } = 5;

    public ObservableCollection<CategoriaPecaDTO> Categorias { get; } = new();
    public ObservableCollection<FornecedorDTO> Fornecedores { get; } = new();
    private Guid? _categoriaIdParaSelecionar;
    private Guid? _fornecedorIdParaSelecionar;
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

    private string _titulo = "Cadastro de Peças";
    public string Titulo
    {
        get => _titulo;
        private set { _titulo = value; OnPropertyChanged(); }
    }

    public ICommand NovaCategoriaCommand { get; }

    public CadastroPecaViewModel(IMediator mediator, Guid? pecaId = null)
    {
        _mediator = mediator;
        _pecaId = pecaId;
        NovaCategoriaCommand = new RelayCommand(async _ => await NovaCategoriaAsync());

        if (pecaId.HasValue)
        {
            Titulo = "Editar Peça";
            CarregarDadosAsync(pecaId.Value);
        }
    }

    private async void CarregarDadosAsync(Guid pecaId)
    {
        var peca = await _mediator.Send(new ObterPecaPorIdQuery { Id = pecaId });
        if (peca != null)
        {
            IdPeca = peca.IdPeca;
            CodigoPeca = peca.CodigoPeca ?? "";
            CodigoBarras = peca.CodigoBarras ?? "";
            Nome = peca.Nome;
            Descricao = peca.Descricao;
            Marca = peca.Marca;
            ValorCusto = peca.ValorCusto;
            ValorVenda = peca.ValorVenda;
            Imposto = peca.Imposto;
            EstoqueInicial = peca.Estoque;
            EstoqueMinimo = peca.EstoqueMinimo;

            _categoriaIdParaSelecionar = peca.CategoriaId;
            _fornecedorIdParaSelecionar = peca.FornecedorId;

            OnPropertyChanged(nameof(IdPeca));
            OnPropertyChanged(nameof(CodigoPeca));
            OnPropertyChanged(nameof(CodigoBarras));
            OnPropertyChanged(nameof(Nome));
            OnPropertyChanged(nameof(Descricao));
            OnPropertyChanged(nameof(Marca));
            OnPropertyChanged(nameof(ValorCusto));
            OnPropertyChanged(nameof(ValorVenda));
            OnPropertyChanged(nameof(Imposto));
            OnPropertyChanged(nameof(EstoqueInicial));
            OnPropertyChanged(nameof(EstoqueMinimo));
        }
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

        if(_categoriaIdParaSelecionar.HasValue)
            CategoriaSelecionada = Categorias.FirstOrDefault(c => c.Id == _categoriaIdParaSelecionar.Value);

        if(_fornecedorIdParaSelecionar.HasValue)
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

    public async Task<bool> SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(IdPeca))
        {
            MessageBox.Show("ID da Peça é obrigatório.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
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
        if (string.IsNullOrWhiteSpace(Marca))
        {
            MessageBox.Show("Marca é obrigatória.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    Marca = Marca,
                    ValorCusto = ValorCusto,
                    ValorVenda = ValorVenda,
                    Imposto = Imposto,
                    CodigoPeca = CodigoPeca,
                    CategoriaId = CategoriaSelecionada?.Id,
                    EstoqueMinimo = EstoqueMinimo
                });
            }
            else
            {
                await _mediator.Send(new CriarPecaCommand
                {
                    IdPeca = IdPeca,
                    Nome = Nome,
                    Descricao = Descricao,
                    Marca = Marca,
                    ValorCusto = ValorCusto,
                    ValorVenda = ValorVenda,
                    Imposto = Imposto,
                    EstoqueInicial = EstoqueInicial,
                    EstoqueMinimo = EstoqueMinimo,
                    CodigoPeca = CodigoPeca,
                    CodigoBarras = CodigoBarras,
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
        IdPeca = string.Empty;
        CodigoPeca = string.Empty;
        CodigoBarras = string.Empty;
        Nome = string.Empty;
        Descricao = string.Empty;
        Marca = string.Empty;
        ValorCusto = 0;
        ValorVenda = 0;
        Imposto = 0;
        EstoqueInicial = 0;
        EstoqueMinimo = 5;
        CategoriaSelecionada = null;
        FornecedorSelecionado = null;

        OnPropertyChanged(nameof(IdPeca));
        OnPropertyChanged(nameof(CodigoPeca));
        OnPropertyChanged(nameof(CodigoBarras));
        OnPropertyChanged(nameof(Nome));
        OnPropertyChanged(nameof(Descricao));
        OnPropertyChanged(nameof(Marca));
        OnPropertyChanged(nameof(ValorCusto));
        OnPropertyChanged(nameof(ValorVenda));
        OnPropertyChanged(nameof(Imposto));
        OnPropertyChanged(nameof(EstoqueInicial));
        OnPropertyChanged(nameof(EstoqueMinimo));
        OnPropertyChanged(nameof(CategoriaSelecionada));
        OnPropertyChanged(nameof(FornecedorSelecionado));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}