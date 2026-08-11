using System.Text.RegularExpressions;
namespace SgaAutoEletrica.Domain.ValueObjects;

/// <summary>
/// Representa um código de barras de produto.
/// Aceita EAN-13, EAN-8 e Code128.
/// </summary>
public class CodigoBarras
{
    public string Valor { get; }
    public TipoCodigoBarras Tipo { get; }

    public CodigoBarras(string valor)
    {
        var limpo = valor.Trim();

        if (!EhValido(limpo))
            throw new ArgumentException("Código de barras inválido");

        Valor = limpo;
        Tipo = DetectarTipo(limpo);
    }

    private static TipoCodigoBarras DetectarTipo(string codigo)
    {
        if (Regex.IsMatch(codigo, @"^\d{8}$"))
            return TipoCodigoBarras.EAN8;

        if (Regex.IsMatch(codigo, @"^\d{13}$"))
            return TipoCodigoBarras.EAN13;

        if (Regex.IsMatch(codigo, @"^\d{14}$"))
            return TipoCodigoBarras.EAN14;

        return TipoCodigoBarras.Outro;
    }

    public static bool EhValido(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            return false;

        if (Regex.IsMatch(codigo, @"^\d{8}$"))
            return true;

        if (Regex.IsMatch(codigo, @"^\d{13}$"))
            return true;

        if (Regex.IsMatch(codigo, @"^\d{14}$"))
            return true;

        // Code128: alfanumérico, até 48 caracteres
        if (Regex.IsMatch(codigo, @"^[A-Za-z0-9\-\.\s]{1,48}$"))
            return true;

        return false;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not CodigoBarras outro)
            return false;
        return Valor == outro.Valor;
    }

    public override int GetHashCode() => Valor.GetHashCode();

    public override string ToString() => Valor;
}

public enum TipoCodigoBarras
{
    EAN8 = 1,
    EAN13 = 2,
    EAN14 = 3,
    Outro = 99  // Code128, QR, etc.
}