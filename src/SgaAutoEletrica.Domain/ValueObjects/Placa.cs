using System.Text.RegularExpressions;

namespace SgaAutoEletrica.Domain.ValueObjects;

/// <summary>
/// Placa de veículo brasileira (padrão antigo ou Mercosul).
/// </summary>
public class Placa
{
    public string Valor { get; }

    public Placa(string valor)
    {
        var limpo = valor.Trim().ToUpperInvariant();

        if (!EhValido(limpo))
            throw new ArgumentException($"Placa inválida: {valor}");

        Valor = limpo;
    }

    /// <summary>
    /// Verifica se é uma placa Mercosul (4 letras + 3 números).
    /// </summary>
    public bool EhMercosul()
    {
        return Regex.IsMatch(Valor, @"^[A-Z]{3}[0-9][A-Z][0-9]{2}$");
    }

    public string Formatada() => Valor;

    public static bool EhValido(string placa)
    {
        if (string.IsNullOrWhiteSpace(placa))
            return false;

        return Regex.IsMatch(placa, @"^[A-Z]{3}[ -]?[0-9]{4}$")  // Antigo
            || Regex.IsMatch(placa, @"^[A-Z]{3}[0-9][A-Z][0-9]{2}$"); // Mercosul
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Placa outro)
            return false;
        return Valor == outro.Valor;
    }

    public override int GetHashCode() => Valor.GetHashCode();

    public override string ToString() => Formatada();
}