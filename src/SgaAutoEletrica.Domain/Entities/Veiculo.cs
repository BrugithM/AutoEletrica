using SgaAutoEletrica.Domain.Enums;
using SgaAutoEletrica.Domain.ValueObjects;

namespace SgaAutoEletrica.Domain.Entities;

/// <summary>
/// Cadastro de veiculos que suporta dois modos de cadastro:
/// - Manual: todos os dados fornecidos pelo usuário
/// - Via API: criado com placa e preenchido depois pela consulta externa
/// </summary>
public class Veiculo
{
    public Guid Id { get; private set; }
    public Placa Placa { get; private set; }
    public string Modelo { get; private set; }
    public string Marca { get; private set; }
    public int Ano { get; private set; }
    public string? Versao { get; private set; }
    public string? Motor { get; private set; }
    public TipoMotor? TipoMotor { get; private set; }
    public string? Cor { get; private set; }
    public string? Observacao { get; private set; }

    public Guid ClienteId { get; private set; }
    public Cliente Cliente { get; private set; } = null!;

    // Construtores 
    private Veiculo()
    {
        Placa = null!;
        Modelo = string.Empty;
        Marca = string.Empty;
    }

    // Construtor para cadastro manual.
    public Veiculo(string placa, Guid clienteId, string modelo, string marca, int ano)
    {
        if (string.IsNullOrWhiteSpace(placa))
            throw new ArgumentException("Placa é obrigatória.", nameof(placa));
        if (string.IsNullOrWhiteSpace(modelo))
            throw new ArgumentException("Modelo é obrigatório.", nameof(modelo));
        if (string.IsNullOrWhiteSpace(marca))
            throw new ArgumentException("Marca é obrigatória.", nameof(marca));
        if (ano < 1950 || ano > DateTime.Now.Year + 1)
            throw new ArgumentException($"Ano inválido: {ano}. Deve estar entre 1950 e {DateTime.Now.Year + 1}.", nameof(ano));

        Id = Guid.NewGuid();
        Placa = new Placa(placa);
        ClienteId = clienteId;
        Modelo = modelo;
        Marca = marca;
        Ano = ano;
    }

    // Construtor para cadastro via API.
    public Veiculo(string placa, Guid clienteId)
    {
        if (string.IsNullOrWhiteSpace(placa))
            throw new ArgumentException("Placa é obrigatória.", nameof(placa));

        Id = Guid.NewGuid();
        Placa = new Placa(placa);
        ClienteId = clienteId;
        Modelo = string.Empty;
        Marca = string.Empty;
        Ano = 0;
    }

    // Métodos de atualização 
    public void PreencherDadosViaApi(
        string modelo,
        string marca,
        int ano,
        string? versao = null,
        string? motor = null,
        TipoMotor? tipoMotor = null,
        string? cor = null)
    {
        if (string.IsNullOrWhiteSpace(modelo))
            throw new ArgumentException("Modelo é obrigatório.", nameof(modelo));
        if (string.IsNullOrWhiteSpace(marca))
            throw new ArgumentException("Marca é obrigatória.", nameof(marca));
        if (ano < 1950 || ano > DateTime.Now.Year + 1)
            throw new ArgumentException($"Ano inválido: {ano}. Deve estar entre 1950 e {DateTime.Now.Year + 1}.", nameof(ano));

        Modelo = modelo;
        Marca = marca;
        Ano = ano;
        Versao = versao;
        Motor = motor;
        TipoMotor = tipoMotor;
        Cor = cor;
    }

    // Atualiza os dados do veículo manualmente.
    public void AtualizarDados(
        string modelo,
        string marca,
        int ano,
        string? versao = null,
        string? motor = null,
        TipoMotor? tipoMotor = null,
        string? cor = null,
        string? observacao = null)
    {
        if (string.IsNullOrWhiteSpace(modelo))
            throw new ArgumentException("Modelo é obrigatório.", nameof(modelo));
        if (string.IsNullOrWhiteSpace(marca))
            throw new ArgumentException("Marca é obrigatória.", nameof(marca));
        if (ano < 1950 || ano > DateTime.Now.Year + 1)
            throw new ArgumentException($"Ano inválido: {ano}. Deve estar entre 1950 e {DateTime.Now.Year + 1}.", nameof(ano));

        Modelo = modelo;
        Marca = marca;
        Ano = ano;
        Versao = versao;
        Motor = motor;
        TipoMotor = tipoMotor;
        Cor = cor;
        Observacao = observacao;
    }
}