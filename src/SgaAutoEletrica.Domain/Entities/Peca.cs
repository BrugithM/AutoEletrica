using SgaAutoEletrica.Domain.ValueObjects;

namespace SgaAutoEletrica.Domain.Entities;

public class Peca
{
    public int Id { get; private set; }
    public string? CodigoPeca { get; private set; }
    public CodigoBarras? CodigoBarras { get; private set; }
    public string Nome { get; private set; }
    public string Descricao { get; private set; }

    public int? MarcaId { get; private set; }
    public Marca? Marca { get; private set; }

    public Guid? CategoriaId { get; private set; }
    public CategoriaPeca? CategoriaPeca { get; private set; }

    public decimal ValorCusto { get; private set; }
    public decimal ValorVenda { get; private set; }
    public decimal MarkupPercentual { get; private set; }

    public int Estoque { get; private set; }
    public int EstoqueMinimo { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public DateTime? DataUltimaAtualizacaoCusto { get; private set; }

    public Guid? FornecedorId { get; private set; }
    public Fornecedor? Fornecedor { get; private set; }

    private Peca()
    {
        Nome = string.Empty;
        Descricao = string.Empty;
    }

    public Peca(
        string nome,
        string descricao,
        decimal valorCusto,
        decimal valorVenda,
        int estoqueInicial,
        int estoqueMinimo = 5,
        string? codigoPeca = null,
        string? codigoBarras = null,
        int? marcaId = null,
        Guid? categoriaId = null,
        Guid? fornecedorId = null
        )
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(nome));
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição é obrigatória.", nameof(descricao));
        if (valorCusto < 0)
            throw new ArgumentException("Valor de custo não pode ser negativo.", nameof(valorCusto));
        if (valorVenda < 0)
            throw new ArgumentException("Valor de venda não pode ser negativo.", nameof(valorVenda));
        if (estoqueInicial < 0)
            throw new ArgumentException("Estoque inicial não pode ser negativo.", nameof(estoqueInicial));

        Nome = nome;
        Descricao = descricao;
        ValorCusto = valorCusto;
        ValorVenda = valorVenda;
        MarkupPercentual = CalcularMarkup(valorCusto, valorVenda);
        Estoque = estoqueInicial;
        EstoqueMinimo = estoqueMinimo;
        CodigoPeca = codigoPeca;
        CodigoBarras = !string.IsNullOrWhiteSpace(codigoBarras) ? new CodigoBarras(codigoBarras) : null;
        MarcaId = marcaId;
        CategoriaId = categoriaId;
        FornecedorId = fornecedorId;
        Ativo = true;
        DataCadastro = DateTime.UtcNow;
    }

    // Cálculos
    public decimal CalcularMargemLucroPercentual()
    {
        if (ValorCusto == 0)
            return 0;
        return Math.Round((ValorVenda - ValorCusto) / ValorVenda * 100, 2);
    }

    private static decimal CalcularMarkup(decimal custo, decimal venda)
    {
        if (custo == 0)
            return 0;
        return Math.Round((venda - custo) / custo * 100, 2);
    }

    // Estoque
    public void DarEntradaEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));
        Estoque += quantidade;
    }

    public void DarBaixaEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));
        if (quantidade > Estoque)
            throw new InvalidOperationException(
                $"Estoque insuficiente. Disponível: {Estoque}, solicitado: {quantidade}.");
        Estoque -= quantidade;
    }

    public bool EstaComEstoqueBaixo() => Estoque <= EstoqueMinimo;

    // Atualizações
    public void AtualizarValorVenda(decimal novoValor)
    {
        if (novoValor < 0)
            throw new ArgumentException("Valor de venda não pode ser negativo.", nameof(novoValor));
        ValorVenda = novoValor;
        MarkupPercentual = CalcularMarkup(ValorCusto, ValorVenda);
    }

    public void AtualizarValorCusto(decimal novoCusto, DateTime dataReferencia)
    {
        if (novoCusto < 0)
            throw new ArgumentException("Valor de custo não pode ser negativo.", nameof(novoCusto));

        if (DataUltimaAtualizacaoCusto == null || dataReferencia >= DataUltimaAtualizacaoCusto.Value)
        {
            ValorCusto = novoCusto;
            DataUltimaAtualizacaoCusto = dataReferencia;
            MarkupPercentual = CalcularMarkup(ValorCusto, ValorVenda);
        }
    }

    public bool PodeAtualizarCusto(DateTime dataReferencia)
    {
        return DataUltimaAtualizacaoCusto == null || dataReferencia >= DataUltimaAtualizacaoCusto.Value;
    }

    public void AtualizarDados(
        string nome,
        string descricao,
        decimal valorCusto,
        decimal valorVenda,
        string? codigoPeca = null,
        int? marcaId = null,
        Guid? categoriaId = null,
        Guid? fornecedorId = null,
        int? estoqueMinimo = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(nome));
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição é obrigatória.", nameof(descricao));
        if (valorCusto < 0)
            throw new ArgumentException("Valor de custo não pode ser negativo.", nameof(valorCusto));

        Nome = nome;
        Descricao = descricao;
        ValorCusto = valorCusto;
        ValorVenda = valorVenda;
        MarkupPercentual = CalcularMarkup(ValorCusto, ValorVenda);
        CodigoPeca = codigoPeca;
        MarcaId = marcaId;
        CategoriaId = categoriaId;
        FornecedorId = fornecedorId;
        if (estoqueMinimo.HasValue)
            EstoqueMinimo = estoqueMinimo.Value;
    }

    public void Desativar() => Ativo = false;
    public void Ativar() => Ativo = true;
}