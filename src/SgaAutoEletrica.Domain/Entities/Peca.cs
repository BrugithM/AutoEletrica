using SgaAutoEletrica.Domain.ValueObjects;

namespace SgaAutoEletrica.Domain.Entities;

public class Peca
{
    public Guid Id{ get; private set; }
    public string IdPeca{ get; private set; }
    public string? CodigoPeca{ get; private set; }
    public CodigoBarras? CodigoBarras{ get; private set; }
    public string Nome{ get; private set; }
    public string Descricao{ get; private set; }
    public string Marca{ get; private set; }

    public Guid? CategoriaId {get; private set;}
    public CategoriaPeca? CategoriaPeca{ get; private set; }

    public decimal ValorCusto{ get; private set; }
    public decimal ValorVenda{ get; private set; }
    public decimal Imposto{ get; private set; }

    public int Estoque { get; private set; }
    public int EstoqueMinimo { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCadastro{ get; private set; }

    public Guid? FornecedorId { get; private set; }
    public Fornecedor? Fornecedor { get; private set; }
    
    private Peca()
    {
        IdPeca = string.Empty;
        Nome = string.Empty;
        Descricao = string.Empty;
        Marca = string.Empty;
    }

    public Peca(
        string idPeca,
        string nome,
        string descricao,
        string marca,
        decimal valorCusto,
        decimal valorVenda,
        decimal imposto,
        int estoqueInicial,
        int estoqueMinimo = 5,
        string? codigoPeca = null,
        string? codigoBarras = null,
        Guid? categoriaId = null,
        Guid? fornecedorId = null
        )
    {
        if (string.IsNullOrWhiteSpace(idPeca))
            throw new ArgumentException("IdPeca é obrigatório.", nameof(idPeca));
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(nome));
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição é obrigatória.", nameof(descricao));
        if (string.IsNullOrWhiteSpace(marca))
            throw new ArgumentException("Marca é obrigatória.", nameof(marca));
        if (valorCusto < 0)
            throw new ArgumentException("Valor de custo não pode ser negativo.", nameof(valorCusto));
        if (valorVenda < 0)
            throw new ArgumentException("Valor de venda não pode ser negativo.", nameof(valorVenda));
        if (imposto < 0)
            throw new ArgumentException("Imposto não pode ser negativo.", nameof(imposto));
        if (estoqueInicial < 0)
            throw new ArgumentException("Estoque inicial não pode ser negativo.", nameof(estoqueInicial));

        Id = Guid.NewGuid();
        IdPeca = idPeca;
        Nome = nome;
        Descricao = descricao;
        Marca = marca;
        ValorCusto = valorCusto;
        ValorVenda = valorVenda;
        Imposto = imposto;
        Estoque = estoqueInicial;
        EstoqueMinimo = estoqueMinimo;
        CodigoPeca = codigoPeca;
        CodigoBarras = !string.IsNullOrWhiteSpace(codigoBarras) ? new CodigoBarras(codigoBarras) : null;
        CategoriaId = categoriaId;
        FornecedorId = fornecedorId;
        Ativo = true;
        DataCadastro = DateTime.UtcNow;
    }

    // Métodos de negócio
    public decimal CalcularMargemLucroPercentual()
    {
        if (ValorCusto == 0)
            return 0;
        return Math.Round((ValorVenda - ValorCusto) / ValorCusto * 100, 2);
    }

    public decimal CalcularPrecoComImposto()
    {
        return Math.Round(ValorVenda * (1 + Imposto / 100), 2);
    }

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
    public void AtualizarValorVenda(decimal novoValor)
    {
        if (novoValor < 0)
            throw new ArgumentException("Valor de venda não pode ser negativo.", nameof(novoValor));
        ValorVenda = novoValor;
    }

    public void AtualizarDados(
        string nome,
        string descricao,
        string marca,
        decimal valorCusto,
        decimal imposto,
        string? codigoPeca = null,
        Guid? categoriaId = null,
        int? estoqueMinimo = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(nome));
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição é obrigatória.", nameof(descricao));
        if (string.IsNullOrWhiteSpace(marca))
            throw new ArgumentException("Marca é obrigatória.", nameof(marca));
        if (valorCusto < 0)
            throw new ArgumentException("Valor de custo não pode ser negativo.", nameof(valorCusto));

        Nome = nome;
        Descricao = descricao;
        Marca = marca;
        ValorCusto = valorCusto;
        Imposto = imposto;
        CodigoPeca = codigoPeca;
        CategoriaId = categoriaId;
        if (estoqueMinimo.HasValue)
            EstoqueMinimo = estoqueMinimo.Value;
    }

    public void Desativar() => Ativo = false;

    public void Ativar() => Ativo = true;
}