namespace SgaAutoEletrica.Domain.ValueObjects;

/// <summary>
/// ValueObject que representa um documento de identificação que pode ser CPF ou CNPJ.
/// Aceita valor nulo/vazio (documento opcional).
/// </summary>
public class CpfCnpj
{
    public string? Valor { get; }

    public bool? EhCpf { get; }

    private CpfCnpj(string? valor, bool? ehCpf)
    {
        Valor = valor;
        EhCpf = ehCpf;
    }

    public static CpfCnpj Criar(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return new CpfCnpj(null, null);

        var apenasNumeros = new string(valor.Where(char.IsDigit).ToArray());

        if (apenasNumeros.Length == 11)
        {
            if (!Cpf.EhValido(apenasNumeros))
                throw new ArgumentException("CPF inválido");
            return new CpfCnpj(apenasNumeros, true);
        }

        if (apenasNumeros.Length == 14)
        {
            if (!Cnpj.EhValido(apenasNumeros))
                throw new ArgumentException("CNPJ inválido");
            return new CpfCnpj(apenasNumeros, false);
        }

        throw new ArgumentException("Documento deve ser CPF (11 dígitos) ou CNPJ (14 dígitos)");
    }

    public string Formatado()
    {
        if (Valor is null)
            return string.Empty;

        return EhCpf == true
            ? new Cpf(Valor).Formatado()
            : new Cnpj(Valor).Formatado();
    }

    public string SemFormatacao() => Valor ?? string.Empty;

    public static bool EhValido(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return true; 

        var apenasNumeros = new string(valor.Where(char.IsDigit).ToArray());

        return apenasNumeros.Length switch
        {
            11 => Cpf.EhValido(apenasNumeros),
            14 => Cnpj.EhValido(apenasNumeros),
            _ => false
        };
    }

    public override bool Equals(object? obj)
    {
        if (obj is not CpfCnpj outro)
            return false;
        return Valor == outro.Valor;
    }

    public override int GetHashCode() => Valor?.GetHashCode() ?? 0;

    public override string ToString() => Formatado();
}