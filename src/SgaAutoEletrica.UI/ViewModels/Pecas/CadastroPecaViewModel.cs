using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
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

    // IDs selecionados (usados com SelectedValue)
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

    private string _titulo = "Nova Peça";
    public string Titulo
    {
        get => _titulo;
        private set { _titulo = value; OnPropertyChanged(); }
    }

    public CadastroPecaViewModel(IMediator mediator, Guid? pecaId = null)
    {
        _mediator = mediator;
        _pecaId = pecaId;

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

            MessageBox.Show(
                $"Erro ao salvar:\n\n{inner.Message}",
                "Erro",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}